using CAMS.Application.Attendance.DTOs;

namespace CAMS.Application.Common.Realtime;

public interface IAttendanceNotifier
{
	Task AttendanceRecordedAsync(
		AttendanceResponse attendance,
		CancellationToken cancellationToken = default);

	Task QrUpdatedAsync(
		AttendanceQrResponse qr,
		CancellationToken cancellationToken = default);
}
