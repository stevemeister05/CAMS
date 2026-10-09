namespace CAMS.Application.Report.DTOs;

public sealed class MemberAttendanceReportRequest
{
	public DateOnly DateFrom { get; set; }

	public DateOnly DateTo { get; set; }

	public Guid? MemberId { get; set; }
}