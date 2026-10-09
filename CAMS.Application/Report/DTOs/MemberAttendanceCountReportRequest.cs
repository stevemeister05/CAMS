namespace CAMS.Application.Report.DTOs;

public sealed class MemberAttendanceCountReportRequest
{
	public DateOnly DateFrom { get; set; }

	public DateOnly DateTo { get; set; }

	public int? MinimumCount { get; set; }

	public int? MaximumCount { get; set; }
}