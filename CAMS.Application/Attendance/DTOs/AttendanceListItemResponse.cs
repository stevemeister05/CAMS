namespace CAMS.Application.Attendance.DTOs;

public class AttendanceListItemResponse
{
	public Guid Id { get; set; }

	public Guid MemberId { get; set; }

	public string MemberName { get; set; } = string.Empty;

	public DateTime TimeIn { get; set; }

	public DateTime? TimeOut { get; set; }
}