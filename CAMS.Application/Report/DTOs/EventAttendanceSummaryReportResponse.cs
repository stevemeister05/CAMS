namespace CAMS.Application.Report.DTOs;

public sealed class EventAttendanceSummaryReportResponse
{
	public Guid EventId { get; set; }

	public string EventName { get; set; } =
		string.Empty;

	public DateOnly EventDate { get; set; }

	public int TotalAttendance { get; set; }
}