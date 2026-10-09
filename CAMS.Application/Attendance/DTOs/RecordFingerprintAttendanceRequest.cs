using CAMS.Domain.Enums;

namespace CAMS.Application.Attendance.DTOs;

public sealed class RecordFingerprintAttendanceRequest
{
	public Guid EventId { get; set; }

	public Guid MemberId { get; set; }

	public AttendanceAction Action { get; set; }
}
