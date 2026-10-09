using System.Security.Cryptography;
using CAMS.Application.Attendance.DTOs;
using CAMS.Application.Common;
using CAMS.Application.Common.Clocking;
using CAMS.Application.Common.Exceptions;
using CAMS.Application.Event;
using CAMS.Application.Settings;
using CAMS.Domain.Entities;
using CAMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CAMS.Application.Attendance;

public class AttendanceQrService : IAttendanceQrService
{
	private readonly IAttendanceQrSessionRepository
		_qrSessionRepository;

	private readonly IEventRepository
		_eventRepository;

	private readonly IUnitOfWork
		_unitOfWork;

	private readonly AttendanceSettings
		_settings;

	private readonly IApplicationClock
		_clock;

	private readonly IAttendanceWindowService
		_attendanceWindowService;


	public AttendanceQrService(
		IAttendanceQrSessionRepository qrSessionRepository,
		IEventRepository eventRepository,
		IUnitOfWork unitOfWork,
		IOptions<AttendanceSettings> settings,
		IApplicationClock clock,
		IAttendanceWindowService attendanceWindowService)
	{
		_qrSessionRepository =
			qrSessionRepository;

		_eventRepository =
			eventRepository;

		_unitOfWork =
			unitOfWork;

		_settings =
			settings.Value;

		_clock =
			clock;

		_attendanceWindowService =
			attendanceWindowService;
	}


	public async Task<AttendanceQrResponse> GetQrCodeAsync(
		Guid eventId,
		AttendanceAction action,
		CancellationToken cancellationToken = default)
	{
		if (
			eventId ==
			Guid.Empty
		)
		{
			throw new ValidationException(
				"Event ID is required.");
		}


		if (!Enum.IsDefined(action))
		{
			throw new ValidationException(
				"Invalid attendance action.");
		}


		// ---------------------------------------------------------
		// Make sure the event exists.
		// ---------------------------------------------------------

		var @event =
			await _eventRepository.GetByIdAsync(
				eventId,
				cancellationToken);


		if (@event is null)
		{
			throw new NotFoundException(
				"Event was not found.");
		}


		// ---------------------------------------------------------
		// Make sure the selected attendance period is open.
		//
		// Event dates and attendance windows use Philippine
		// local time.
		//
		// IMPORTANT:
		// This happens BEFORE looking for or generating a QR.
		// Therefore an existing QR cannot be returned outside
		// its attendance window either.
		// ---------------------------------------------------------

		_attendanceWindowService.Validate(
			@event,
			action,
			_clock.LocalNow);


		/*
		 * QR token timestamps are stored and compared using UTC.
		 */
		var now =
			_clock.UtcNow;


		// ---------------------------------------------------------
		// Look for the current QR session.
		// ---------------------------------------------------------

		var session =
			await _qrSessionRepository
				.GetByEventAndActionAsync(
					eventId,
					action,
					cancellationToken);


		// ---------------------------------------------------------
		// Existing session.
		// ---------------------------------------------------------

		if (session is not null)
		{
			/*
			 * The existing QR code is still valid.
			 *
			 * Reuse it rather than generating a new token.
			 */
			if (
				session.ExpiresAt >
				now
			)
			{
				return MapToResponse(
					session);
			}


			/*
			 * The QR code has expired.
			 *
			 * Since we don't retain QR history, simply
			 * replace the token and expiration information
			 * on the existing row.
			 *
			 * We already validated the attendance window
			 * above, so rotation can only occur while the
			 * selected attendance period is open.
			 */
			session.Token =
				GenerateToken();


			session.CreatedAt =
				now;


			session.ExpiresAt =
				now.AddSeconds(
					_settings
						.QrCodeExpirationSeconds);


			await _unitOfWork.SaveChangesAsync(
				cancellationToken);


			return MapToResponse(
				session);
		}


		// ---------------------------------------------------------
		// No existing session.
		// ---------------------------------------------------------

		session =
			new AttendanceQrSession
			{
				EventId =
					eventId,

				Action =
					action,

				Token =
					GenerateToken(),

				CreatedAt =
					now,

				ExpiresAt =
					now.AddSeconds(
						_settings
							.QrCodeExpirationSeconds)
			};


		await _qrSessionRepository.AddAsync(
			session,
			cancellationToken);


		try
		{
			await _unitOfWork.SaveChangesAsync(
				cancellationToken);
		}
		catch (DbUpdateException)
		{
			/*
			 * Another request may have created the
			 * session at the same time.
			 *
			 * The unique EventId + Action index protects
			 * us from creating duplicate sessions.
			 */
			var existingSession =
				await _qrSessionRepository
					.GetByEventAndActionAsync(
						eventId,
						action,
						cancellationToken);


			if (
				existingSession is not null &&
				existingSession.ExpiresAt >
					_clock.UtcNow
			)
			{
				return MapToResponse(
					existingSession);
			}


			throw;
		}


		return MapToResponse(
			session);
	}


	public async Task<ValidateAttendanceQrResponse> ValidateQrCodeAsync(
		string token,
		CancellationToken cancellationToken = default)
	{
		if (
			string.IsNullOrWhiteSpace(
				token)
		)
		{
			throw new ValidationException(
				"QR token is required.");
		}


		token =
			token.Trim();


		var session =
			await _qrSessionRepository.GetByTokenAsync(
				token,
				cancellationToken);


		if (session is null)
		{
			throw new ValidationException(
				"The QR code is invalid.");
		}


		var now =
			_clock.UtcNow;


		if (
			session.ExpiresAt <=
			now
		)
		{
			throw new ValidationException(
				"The QR code has expired.");
		}


		// ---------------------------------------------------------
		// Make sure the associated event still exists.
		// ---------------------------------------------------------

		var @event =
			await _eventRepository.GetByIdAsync(
				session.EventId,
				cancellationToken);


		if (@event is null)
		{
			throw new NotFoundException(
				"The event associated with the QR code was not found.");
		}


		// ---------------------------------------------------------
		// Make sure the action is valid.
		// ---------------------------------------------------------

		if (
			!Enum.IsDefined(
				session.Action)
		)
		{
			throw new ValidationException(
				"The QR code contains an invalid attendance action.");
		}


		// ---------------------------------------------------------
		// Make sure the QR is being used during the correct
		// attendance period.
		//
		// A token may technically still have a few seconds left
		// before ExpiresAt even though the event's attendance
		// window has just closed. This prevents that token from
		// being accepted.
		// ---------------------------------------------------------

		_attendanceWindowService.Validate(
			@event,
			session.Action,
			_clock.LocalNow);


		return new ValidateAttendanceQrResponse
		{
			EventId =
				session.EventId,

			Action =
				session.Action,

			ExpiresAt =
				session.ExpiresAt
		};
	}


	private static string GenerateToken()
	{
		var bytes =
			RandomNumberGenerator.GetBytes(
				32);


		return Convert
			.ToBase64String(
				bytes)
			.Replace(
				"+",
				"-")
			.Replace(
				"/",
				"_")
			.TrimEnd(
				'=');
	}


	private static AttendanceQrResponse MapToResponse(
		AttendanceQrSession session)
	{
		return new AttendanceQrResponse
		{
			EventId =
				session.EventId,

			Action =
				session.Action,

			Token =
				session.Token,

			CreatedAt =
				session.CreatedAt,

			ExpiresAt =
				session.ExpiresAt
		};
	}
}