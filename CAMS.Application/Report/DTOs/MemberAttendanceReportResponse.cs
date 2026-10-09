using CAMS.Domain.Enums;

namespace CAMS.Application.Report.DTOs;

public sealed class MemberAttendanceReportResponse
{
	public Guid AttendanceId { get; set; }

	public Guid MemberId { get; set; }

	public string MemberName { get; set; } =
		string.Empty;

	public Guid EventId { get; set; }

	public string EventName { get; set; } =
		string.Empty;

	public DateOnly EventDate { get; set; }

	public DateTime? TimeIn { get; set; }

	public DateTime? TimeOut { get; set; }

	public AttendanceMethod? TimeInMethod { get; set; }

	public AttendanceMethod? TimeOutMethod { get; set; }
}