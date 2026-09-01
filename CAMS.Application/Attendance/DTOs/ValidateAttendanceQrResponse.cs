using CAMS.Domain.Enums;

namespace CAMS.Application.Attendance.DTOs;

public class ValidateAttendanceQrResponse
{
	public Guid EventId { get; set; }

	public AttendanceAction Action { get; set; }

	public DateTime ExpiresAt { get; set; }
}
