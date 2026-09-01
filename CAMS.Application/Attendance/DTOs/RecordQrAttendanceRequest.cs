namespace CAMS.Application.Attendance.DTOs;

public sealed class RecordQrAttendanceRequest
{
	public Guid EventId { get; set; }

	public string Token { get; set; } = string.Empty;
}
