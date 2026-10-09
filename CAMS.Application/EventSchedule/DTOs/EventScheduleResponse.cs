using CAMS.Domain.Enums;

namespace CAMS.Application.EventSchedule.DTOs;

public class EventScheduleResponse
{
	public Guid Id { get; set; }

	public string Name { get; set; } = string.Empty;

	public EventType EventType { get; set; }

	public bool IsActive { get; set; }

	public TimeOnly StartTime { get; set; }
	public TimeOnly EndTime { get; set; }

	public TimeOnly AttendanceTimeInStart { get; set; }
	public TimeOnly AttendanceTimeInEnd { get; set; }

	public TimeOnly AttendanceTimeOutStart { get; set; }
	public TimeOnly AttendanceTimeOutEnd { get; set; }

	public DayOfWeek? DayOfWeek { get; set; }

	public int? StartMonth { get; set; }
	public int? StartDay { get; set; }

	public int? EndMonth { get; set; }
	public int? EndDay { get; set; }

	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
