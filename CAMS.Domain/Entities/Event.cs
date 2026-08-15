using CAMS.Domain.Enums;

namespace CAMS.Domain.Entities;

public class Event : BaseEntity
{
	public Event() : base() { }

	public string Name { get; set; } = string.Empty;

	public EventType Type { get; set; }

	public DateOnly EventDate { get; set; }

	public TimeOnly StartTime { get; set; }
	public TimeOnly EndTime { get; set; }

	public TimeOnly AttendanceTimeInStart { get; set; }
	public TimeOnly AttendanceTimeInEnd { get; set; }

	public TimeOnly AttendanceTimeOutStart { get; set; }
	public TimeOnly AttendanceTimeOutEnd { get; set; }

	public EventStatus Status { get; set; }

	public string? Description { get; set; }

	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }

	public ICollection<Attendance> Attendances { get; set; } = [];
}