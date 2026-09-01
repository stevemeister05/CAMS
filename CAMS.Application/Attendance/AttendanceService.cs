using CAMS.Application.Attendance.DTOs;
using CAMS.Application.Common;
using CAMS.Application.Common.Clocking;
using CAMS.Application.Common.Exceptions;
using CAMS.Application.Common.Realtime;
using CAMS.Application.Event;
using CAMS.Application.Member;
using CAMS.Domain.Entities;
using CAMS.Domain.Enums;
using System.Linq.Expressions;

namespace CAMS.Application.Attendance;

public class AttendanceService : IAttendanceService
{
	private readonly IAttendanceRepository _attendanceRepository;
	private readonly IAttendanceQrSessionRepository _qrSessionRepository;
	private readonly IEventRepository _eventRepository;
	private readonly IMemberRepository _memberRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IApplicationClock _clock;
	private readonly IAttendanceNotifier _attendanceNotifier;
	private readonly IAttendanceWindowService _attendanceWindowService;

	public AttendanceService(
		IAttendanceRepository attendanceRepository,
		IAttendanceQrSessionRepository qrSessionRepository,
		IEventRepository eventRepository,
		IMemberRepository memberRepository,
		IUnitOfWork unitOfWork,
		IApplicationClock clock,
		IAttendanceNotifier attendanceNotifier,
		IAttendanceWindowService attendanceWindowService)
	{
		_attendanceRepository = attendanceRepository;
		_qrSessionRepository = qrSessionRepository;
		_eventRepository = eventRepository;
		_memberRepository = memberRepository;
		_unitOfWork = unitOfWork;
		_clock = clock;
		_attendanceNotifier = attendanceNotifier;
		_attendanceWindowService = attendanceWindowService;
	}

	public async Task<AttendancePageResponse> GetAttendanceByEventAsync(
		Guid eventId,
		CancellationToken cancellationToken = default)
	{
		ValidateEventId(
			eventId);


		var @event =
			await _eventRepository.GetByIdAsync(
				eventId,
				cancellationToken);


		if (@event is null)
		{
			throw new NotFoundException(
				"Event was not found.");
		}


		var attendances =
			await _attendanceRepository.GetByEventAsync(
				eventId,
				cancellationToken);


		var qrAction = AttendanceAction.TimeIn;

		AttendanceQrSession? qr =
			null;


		var localNow =
			_clock.LocalNow;


		if (
			_attendanceWindowService.IsOpen(
				@event,
				qrAction,
				localNow)
		)
		{
			qr =
				await _qrSessionRepository
					.GetByEventAndActionAsync(
						eventId,
						qrAction,
						cancellationToken);


			/*
			 * Don't return an already-expired QR
			 * during initial page loading.
			 */
			if (
				qr is not null &&
				qr.ExpiresAt <=
					_clock.UtcNow
			)
			{
				qr =
					null;
			}
		}


		return new AttendancePageResponse
		{
			Event =
				new AttendanceEventResponse
				{
					Id =
						@event.Id,

					Name =
						@event.Name,

					EventDate =
						@event.EventDate,

					AttendanceTimeInStart =
						@event.AttendanceTimeInStart,

					AttendanceTimeInEnd =
						@event.AttendanceTimeInEnd,

					AttendanceTimeOutStart =
						@event.AttendanceTimeOutStart,

					AttendanceTimeOutEnd =
						@event.AttendanceTimeOutEnd
				},

			QrAction =
				qrAction,

			Qr =
				qr is null
					? null
					: MapToQrResponse(
						qr),

			Attendances =
				attendances
					.Select(
						MapToAttendanceListItem)
					.ToList()
		};
	}

	public async Task<AttendanceResponse> RecordQrAttendanceAsync(
		Guid memberId,
		RecordQrAttendanceRequest request,
		CancellationToken cancellationToken = default)
	{
		ValidateMemberId(memberId);

		if (request is null)
		{
			throw new ValidationException(
				"Attendance information is required.");
		}

		ValidateEventId(request.EventId);

		if (string.IsNullOrWhiteSpace(request.Token))
		{
			throw new ValidationException(
				"QR code token is required.");
		}

		var token =
			request.Token.Trim();

		// Validate QR session
		var qrSession =
			await _qrSessionRepository.GetByTokenAsync(
				token,
				cancellationToken);

		if (qrSession is null)
		{
			throw new ConflictException(
				"The QR code is invalid or has expired.");
		}

		if (qrSession.EventId != request.EventId)
		{
			throw new ConflictException(
				"The QR code does not belong to this event.");
		}

		var nowUtc =
			_clock.UtcNow;

		if (qrSession.ExpiresAt <= nowUtc)
		{
			throw new ConflictException(
				"The QR code has expired.");
		}

		return await RecordAttendanceAsync(
			memberId: memberId,
			eventId: qrSession.EventId,
			action: qrSession.Action,
			method: AttendanceMethod.QRCode,
			cancellationToken: cancellationToken);
	}

	public async Task<AttendanceResponse>
		RecordFingerprintAttendanceAsync(
			RecordFingerprintAttendanceRequest request,
			CancellationToken cancellationToken = default)
	{
		if (request is null)
		{
			throw new ValidationException(
				"Attendance information is required.");
		}

		ValidateMemberId(request.MemberId);

		ValidateEventId(request.EventId);

		ValidateAction(request.Action);

		return await RecordAttendanceAsync(
			memberId: request.MemberId,
			eventId: request.EventId,
			action: request.Action,
			method: AttendanceMethod.Fingerprint,
			cancellationToken: cancellationToken);
	}

	public async Task<AttendanceResponse>
		RecordManualAttendanceAsync(
			RecordManualAttendanceRequest request,
			CancellationToken cancellationToken = default)
	{
		if (request is null)
		{
			throw new ValidationException(
				"Attendance information is required.");
		}

		ValidateMemberId(request.MemberId);

		ValidateEventId(request.EventId);

		ValidateAction(request.Action);

		return await RecordAttendanceAsync(
			memberId: request.MemberId,
			eventId: request.EventId,
			action: request.Action,
			method: AttendanceMethod.Manual,
			cancellationToken: cancellationToken);
	}
	

	// COMMON ATTENDANCE WORKFLOW
	private async Task<AttendanceResponse>
		RecordAttendanceAsync(
			Guid memberId,
			Guid eventId,
			AttendanceAction action,
			AttendanceMethod method,
			CancellationToken cancellationToken)
	{
		var member =
			await _memberRepository.GetByIdAsync(
				memberId,
				cancellationToken);

		if (member is null)
		{
			throw new NotFoundException(
				"Member was not found.");
		}

		if (!member.IsActive)
		{
			throw new ConflictException(
				"This member is inactive.");
		}

		var @event =
			await _eventRepository.GetByIdAsync(
				eventId,
				cancellationToken);

		if (@event is null)
		{
			throw new NotFoundException(
				"Event was not found.");
		}

		// Event/attendance windows use Philippine local time.
		var localNow =
			_clock.LocalNow;

		_attendanceWindowService.Validate(
			@event,
			action,
			localNow);

		// Attendance timestamps are stored as UTC.
		var utcNow =
			_clock.UtcNow;

		var attendance =
			await _attendanceRepository
				.GetByMemberAndEventAsync(
					memberId,
					eventId,
					cancellationToken);

		if (action ==
			AttendanceAction.TimeIn)
		{
			return await RecordTimeInAsync(
				member,
				eventId,
				method,
				utcNow,
				attendance,
				cancellationToken);
		}

		if (action ==
			AttendanceAction.TimeOut)
		{
			return await RecordTimeOutAsync(
				member,
				attendance,
				method,
				utcNow,
				cancellationToken);
		}

		throw new ValidationException(
			"Invalid attendance action.");
	}

	// TIME-IN
	private async Task<AttendanceResponse> RecordTimeInAsync(
		Domain.Entities.Member member,
		Guid eventId,
		AttendanceMethod method,
		DateTime utcNow,
		Domain.Entities.Attendance? attendance,
		CancellationToken cancellationToken)
	{
		if (attendance is not null)
		{
			throw new ConflictException(
				"Attendance has already been recorded " +
				"for this event.");
		}

		attendance =
			new Domain.Entities.Attendance
			{
				MemberId =
					member.Id,

				EventId =
					eventId,

				TimeIn =
					utcNow,

				TimeInMethod =
					method
			};

		await _attendanceRepository.AddAsync(
			attendance,
			cancellationToken);

		await _unitOfWork.SaveChangesAsync(
			cancellationToken);

		var response = MapToResponse(
			attendance,
			AttendanceAction.TimeIn,
			member.GetFullNameLastFirst());

		await _attendanceNotifier.AttendanceRecordedAsync(
			response,
			cancellationToken);

		return response;
	}

	// TIME-OUT
	private async Task<AttendanceResponse> RecordTimeOutAsync(
			Domain.Entities.Member member,
			Domain.Entities.Attendance? attendance,
			AttendanceMethod method,
			DateTime utcNow,
			CancellationToken cancellationToken)
	{
		if (attendance is null)
		{
			throw new ConflictException(
				"Time-out cannot be recorded because " +
				"the member has not timed in.");
		}

		if (attendance.TimeOut is not null)
		{
			throw new ConflictException(
				"Time-out has already been recorded " +
				"for this event.");
		}

		attendance.TimeOut =
			utcNow;

		attendance.TimeOutMethod =
			method;

		attendance.UpdatedAt =
			utcNow;

		_attendanceRepository.Update(
			attendance);

		await _unitOfWork.SaveChangesAsync(
			cancellationToken);

		var response = MapToResponse(
			attendance,
			AttendanceAction.TimeOut,
			member.GetFullNameLastFirst());

		await _attendanceNotifier.AttendanceRecordedAsync(
			response,
			cancellationToken);

		return response;
	}

	// VALIDATION
	private static void ValidateMemberId(
		Guid memberId)
	{
		if (memberId == Guid.Empty)
		{
			throw new ValidationException(
				"Member ID is required.");
		}
	}

	private static void ValidateEventId(
		Guid eventId)
	{
		if (eventId == Guid.Empty)
		{
			throw new ValidationException(
				"Event ID is required.");
		}
	}

	private static void ValidateAction(
		AttendanceAction action)
	{
		if (!Enum.IsDefined(action))
		{
			throw new ValidationException(
				"Invalid attendance action.");
		}
	}

	// RESPONSE
	private static AttendanceResponse MapToResponse(
		Domain.Entities.Attendance attendance,
		AttendanceAction action,
		string memberName)
	{
		return new AttendanceResponse
		{
			Id = attendance.Id,
			MemberId = attendance.MemberId,
			MemberName = memberName,
			EventId = attendance.EventId,
			TimeIn = attendance.TimeIn,
			TimeOut = attendance.TimeOut,
			TimeInMethod = attendance.TimeInMethod,
			TimeOutMethod = attendance.TimeOutMethod,
			Action = action,
			UpdatedAt = attendance.UpdatedAt
		};
	}

	private static AttendanceQrResponse MapToQrResponse(
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

	private static AttendanceListItemResponse MapToAttendanceListItem(
		Domain.Entities.Attendance attendance)
	{
		return new AttendanceListItemResponse
		{
			Id =
				attendance.Id,

			MemberId =
				attendance.MemberId,

			MemberName =
				BuildMemberName(attendance.Member),

			TimeIn =
				attendance.TimeIn,

			TimeOut =
				attendance.TimeOut
		};
	}

	private static string BuildMemberName(
		Domain.Entities.Member member)
	{
		return string.Join(
			" ",
			new[]
			{
			member.FirstName,
			member.MiddleName,
			member.LastName
			}
			.Where(x =>
				!string.IsNullOrWhiteSpace(x)));
	}

	private static bool IsAttendanceWindowOpen(
	Domain.Entities.Event @event,
	AttendanceAction action,
	DateTime localNow)
	{
		var currentDate =
			DateOnly.FromDateTime(
				localNow);


		if (
			@event.EventDate !=
			currentDate
		)
		{
			return false;
		}


		var currentTime =
			TimeOnly.FromDateTime(
				localNow);


		if (
			action ==
			AttendanceAction.TimeIn
		)
		{
			return
				currentTime >=
					@event.AttendanceTimeInStart &&
				currentTime <=
					@event.AttendanceTimeInEnd;
		}


		if (
			action ==
			AttendanceAction.TimeOut
		)
		{
			return
				currentTime >=
					@event.AttendanceTimeOutStart &&
				currentTime <=
					@event.AttendanceTimeOutEnd;
		}


		return false;
	}
}
