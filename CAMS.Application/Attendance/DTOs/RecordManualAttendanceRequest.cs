using CAMS.Domain.Enums;

public sealed class RecordManualAttendanceRequest
{
	public Guid EventId { get; set; }

	public Guid MemberId { get; set; }

	public AttendanceAction Action { get; set; }
}
