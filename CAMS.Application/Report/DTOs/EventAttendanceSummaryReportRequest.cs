namespace CAMS.Application.Report.DTOs;

public sealed class EventAttendanceSummaryReportRequest
{
	public DateOnly DateFrom { get; set; }

	public DateOnly DateTo { get; set; }
}