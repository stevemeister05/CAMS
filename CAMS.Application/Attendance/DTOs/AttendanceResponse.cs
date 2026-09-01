using CAMS.Domain.Enums;

namespace CAMS.Application.Attendance.DTOs;

public sealed class AttendanceResponse
{
	public Guid Id { get; set; }

	public Guid MemberId { get; set; }

	public string MemberName { get; set; } = string.Empty;

	public Guid EventId { get; set; }

	public DateTime TimeIn { get; set; }

	public DateTime? TimeOut { get; set; }

	public AttendanceMethod TimeInMethod { get; set; }

	public AttendanceMethod? TimeOutMethod { get; set; }

	public AttendanceAction Action { get; set; }

	public DateTime? UpdatedAt { get; set; }
}
