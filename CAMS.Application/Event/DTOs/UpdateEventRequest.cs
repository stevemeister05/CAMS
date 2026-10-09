using CAMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Application.Event.DTOs;

public class UpdateEventRequest
{
	public string Name { get; set; } = string.Empty;

	public EventType Type { get; set; }

	public DateOnly EventDate { get; set; }

	public TimeOnly StartTime { get; set; }
	public TimeOnly EndTime { get; set; }

	public TimeOnly AttendanceTimeInStart { get; set; }
	public TimeOnly AttendanceTimeInEnd { get; set; }

	public TimeOnly AttendanceTimeOutStart { get; set; }
	public TimeOnly AttendanceTimeOutEnd { get; set; }

	public string? Description { get; set; }
}
