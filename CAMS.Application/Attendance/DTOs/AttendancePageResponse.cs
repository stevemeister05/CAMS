using CAMS.Domain.Enums;

namespace CAMS.Application.Attendance.DTOs;

public class AttendancePageResponse
{
	public AttendanceEventResponse Event { get; set; } = null!;

	public AttendanceAction QrAction { get; set; }

	public AttendanceQrResponse? Qr { get; set; }

	public IReadOnlyList<AttendanceListItemResponse>
		Attendances
	{ get; set; } =
			Array.Empty<AttendanceListItemResponse>();
}