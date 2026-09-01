namespace CAMS.Application.Attendance.DTOs;

public class AttendanceEventResponse
{
	public Guid Id { get; set; }

	public string Name { get; set; } = string.Empty;

	public DateOnly EventDate { get; set; }

	public TimeOnly AttendanceTimeInStart { get; set; }

	public TimeOnly AttendanceTimeInEnd { get; set; }

	public TimeOnly AttendanceTimeOutStart { get; set; }

	public TimeOnly AttendanceTimeOutEnd { get; set; }
}