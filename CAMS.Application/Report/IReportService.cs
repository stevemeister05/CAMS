using CAMS.Application.Report.DTOs;

namespace CAMS.Application.Report;

public interface IReportService
{
	Task<IReadOnlyList<MemberAttendanceReportResponse>> GetMemberAttendanceAsync(
		MemberAttendanceReportRequest request,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<MemberAttendanceCountReportResponse>> GetMemberAttendanceCountAsync(
		MemberAttendanceCountReportRequest request,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<EventAttendanceSummaryReportResponse>> GetEventAttendanceSummaryAsync(
		EventAttendanceSummaryReportRequest request,
		CancellationToken cancellationToken = default);

	Task<IndividualEventAttendanceReportResponse> GetIndividualEventAttendanceAsync(
		IndividualEventAttendanceReportRequest request,
		CancellationToken cancellationToken = default);
}