namespace CAMS.Application.Report.DTOs;

public sealed class MemberAttendanceCountReportResponse
{
	public Guid MemberId { get; set; }

	public string MemberName { get; set; } =
		string.Empty;

	public int AttendanceCount { get; set; }
}
