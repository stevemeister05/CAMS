using CAMS.Domain.Enums;

namespace CAMS.Application.Attendance.DTOs;

public sealed class AttendanceQrResponse
{
	public Guid EventId { get; set; }

	public AttendanceAction Action { get; set; }

	public string Token { get; set; } = string.Empty;

	public DateTime CreatedAt { get; set; }

	public DateTime ExpiresAt { get; set; }
}
