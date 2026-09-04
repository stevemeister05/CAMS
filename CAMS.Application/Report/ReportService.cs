using CAMS.Application.Attendance;
using CAMS.Application.Common.Exceptions;
using CAMS.Application.Report.DTOs;

namespace CAMS.Application.Report;

public sealed class ReportService
	: IReportService
{
	private readonly IReportRepository
		_reportRepository;
	private readonly IAttendanceService _attendanceService;

	public ReportService(IReportRepository reportRepository, IAttendanceService attendanceService)
	{
		_reportRepository =
			reportRepository;

		_attendanceService =
			attendanceService;
	}


	public async Task<
		IReadOnlyList<MemberAttendanceReportResponse>>
		GetMemberAttendanceAsync(
			MemberAttendanceReportRequest request,
			CancellationToken cancellationToken = default)
	{
		ValidateRequest(
			request);


		var attendances =
			await _reportRepository
				.GetMemberAttendanceAsync(
					request.DateFrom,
					request.DateTo,
					cancellationToken);


		return attendances
			.Select(MapToMemberAttendance)
			.ToList();
	}

	public async Task<IReadOnlyList<MemberAttendanceCountReportResponse>> GetMemberAttendanceCountAsync(
		MemberAttendanceCountReportRequest request,
		CancellationToken cancellationToken = default)
	{
		ValidateMemberAttendanceCountRequest(
			request);


		return await _reportRepository
			.GetMemberAttendanceCountAsync(
				request.DateFrom,
				request.DateTo,
				request.MinimumCount,
				request.MaximumCount,
				cancellationToken);
	}

	public async Task<IReadOnlyList<EventAttendanceSummaryReportResponse>> GetEventAttendanceSummaryAsync(
		EventAttendanceSummaryReportRequest request,
		CancellationToken cancellationToken = default)
	{
		ValidateEventAttendanceSummaryRequest(
			request);


		return await _reportRepository
			.GetEventAttendanceSummaryAsync(
				request.DateFrom,
				request.DateTo,
				cancellationToken);
	}

	public async Task<IndividualEventAttendanceReportResponse> GetIndividualEventAttendanceAsync(
		IndividualEventAttendanceReportRequest request,
		CancellationToken cancellationToken = default)
	{
		ValidateIndividualEventAttendanceRequest(
			request);


		var attendance =
			await _attendanceService
				.GetAttendanceByEventAsync(
					request.EventId,
					cancellationToken);


		return new IndividualEventAttendanceReportResponse
		{
			EventId =
				attendance.Event.Id,

			EventName =
				attendance.Event.Name,

			EventDate =
				attendance.Event.EventDate,

			Attendances =
				attendance.Attendances
					.Select(item =>
						new IndividualEventAttendanceReportItemResponse
						{
							AttendanceId =
								item.Id,

							MemberId =
								item.MemberId,

							MemberName =
								item.MemberName,

							TimeIn =
								AsUtc(
									item.TimeIn),

							TimeOut =
								AsUtc(
									item.TimeOut)
						})
					.OrderBy(item =>
						item.MemberName)
					.ToList()
		};
	}

	private static void ValidateIndividualEventAttendanceRequest(
		IndividualEventAttendanceReportRequest request)
	{
		if (request is null)
		{
			throw new ValidationException(
				"Report filters are required.");
		}


		if (request.EventId == Guid.Empty)
		{
			throw new ValidationException(
				"Event is required.");
		}
	}

	private static void ValidateEventAttendanceSummaryRequest(EventAttendanceSummaryReportRequest request)
	{
		if (request is null)
		{
			throw new ValidationException(
				"Report filters are required.");
		}


		if (
			request.DateFrom ==
			default
		)
		{
			throw new ValidationException(
				"Date From is required.");
		}


		if (
			request.DateTo ==
			default
		)
		{
			throw new ValidationException(
				"Date To is required.");
		}


		if (
			request.DateFrom >
			request.DateTo
		)
		{
			throw new ValidationException(
				"Date From cannot be later than Date To.");
		}
	}

	private static void ValidateMemberAttendanceCountRequest(
		MemberAttendanceCountReportRequest request)
	{
		if (request is null)
		{
			throw new ValidationException(
				"Report filters are required.");
		}


		if (
			request.DateFrom ==
			default
		)
		{
			throw new ValidationException(
				"Date From is required.");
		}


		if (
			request.DateTo ==
			default
		)
		{
			throw new ValidationException(
				"Date To is required.");
		}


		if (
			request.DateFrom >
			request.DateTo
		)
		{
			throw new ValidationException(
				"Date From cannot be later than Date To.");
		}


		if (
			request.MinimumCount.HasValue &&
			request.MinimumCount.Value < 0
		)
		{
			throw new ValidationException(
				"Minimum attendance count cannot be negative.");
		}


		if (
			request.MaximumCount.HasValue &&
			request.MaximumCount.Value < 0
		)
		{
			throw new ValidationException(
				"Maximum attendance count cannot be negative.");
		}


		if (
			request.MinimumCount.HasValue &&
			request.MaximumCount.HasValue &&
			request.MinimumCount.Value >
			request.MaximumCount.Value
		)
		{
			throw new ValidationException(
				"Minimum attendance count cannot be " +
				"greater than maximum attendance count.");
		}
	}


	private static MemberAttendanceReportResponse MapToMemberAttendance(
		Domain.Entities.Attendance attendance)
	{
		return new MemberAttendanceReportResponse
		{
			AttendanceId =
				attendance.Id,

			MemberId =
				attendance.MemberId,

			MemberName =
				attendance.Member
					.GetFullNameLastFirst(),

			EventId =
				attendance.EventId,

			EventName =
				attendance.Event.Name,

			EventDate =
				attendance.Event.EventDate,

			TimeIn =
				AsUtc(
					attendance.TimeIn),

			TimeOut =
				AsUtc(
					attendance.TimeOut)
		};
	}


	private static DateTime? AsUtc(
		DateTime? value)
	{
		if (!value.HasValue)
		{
			return null;
		}


		return DateTime.SpecifyKind(
			value.Value,
			DateTimeKind.Utc);
	}


	private static void ValidateRequest(
		MemberAttendanceReportRequest request)
	{
		if (request is null)
		{
			throw new ValidationException(
				"Report filters are required.");
		}


		if (
			request.DateFrom ==
			default
		)
		{
			throw new ValidationException(
				"Date From is required.");
		}


		if (
			request.DateTo ==
			default
		)
		{
			throw new ValidationException(
				"Date To is required.");
		}


		if (
			request.DateFrom >
			request.DateTo
		)
		{
			throw new ValidationException(
				"Date From cannot be later than Date To.");
		}
	}
}