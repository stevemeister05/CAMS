using CAMS.Domain.Enums;

namespace CAMS.Domain.Entities;

public class EventSchedule : BaseEntity
{
	public EventSchedule() : base() { }

	public string Name { get; set; } = string.Empty;

	public EventType EventType { get; set; }

	public bool IsActive { get; set; }

	public TimeOnly StartTime { get; set; }
	public TimeOnly EndTime { get; set; }

	public TimeOnly AttendanceTimeInStart { get; set; }
	public TimeOnly AttendanceTimeInEnd { get; set; }

	public TimeOnly AttendanceTimeOutStart { get; set; }
	public TimeOnly AttendanceTimeOutEnd { get; set; }

	// Used for recurring schedules
	public DayOfWeek? DayOfWeek { get; set; }

	// Used for seasonal schedules such as Misa de Gallo
	public DateOnly? StartDate { get; set; }
	public DateOnly? EndDate { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
