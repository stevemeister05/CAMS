namespace CAMS.Application.Report.DTOs;

public sealed class IndividualEventAttendanceReportResponse
{
	public Guid EventId { get; set; }

	public string EventName { get; set; } =
		string.Empty;

	public DateOnly EventDate { get; set; }

	public IReadOnlyList<IndividualEventAttendanceReportItemResponse>
		Attendances
	{ get; set; } =
			[];
}


public sealed class IndividualEventAttendanceReportItemResponse
{
	public Guid AttendanceId { get; set; }

	public Guid MemberId { get; set; }

	public string MemberName { get; set; } =
		string.Empty;

	public DateTime? TimeIn { get; set; }

	public DateTime? TimeOut { get; set; }
}