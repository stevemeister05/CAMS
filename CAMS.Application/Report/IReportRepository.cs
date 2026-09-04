using CAMS.Application.Report.DTOs;

namespace CAMS.Application.Report;

public interface IReportRepository
{
	Task<IReadOnlyList<Domain.Entities.Attendance>> GetMemberAttendanceAsync(
		DateOnly dateFrom,
		DateOnly dateTo,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<MemberAttendanceCountReportResponse>> GetMemberAttendanceCountAsync(
		DateOnly dateFrom,
		DateOnly dateTo,
		int? minimumCount,
		int? maximumCount,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<EventAttendanceSummaryReportResponse>> GetEventAttendanceSummaryAsync(
		DateOnly dateFrom,
		DateOnly dateTo,
		CancellationToken cancellationToken = default);
}