using CAMS.Domain.Enums;

namespace CAMS.Application.Attendance.DTOs;

public sealed class MemberAttendanceHistoryResponse
{
	public Guid Id { get; set; }

	public Guid EventId { get; set; }

	public string EventName { get; set; } =
		string.Empty;

	public DateOnly EventDate { get; set; }

	public DateTime? TimeIn { get; set; }

	public DateTime? TimeOut { get; set; }

	public AttendanceMethod? TimeInMethod { get; set; }

	public AttendanceMethod? TimeOutMethod { get; set; }
}
