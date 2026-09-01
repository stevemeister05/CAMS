using CAMS.Application.Attendance.DTOs;
using CAMS.Domain.Enums;

namespace CAMS.Application.Attendance;

public interface IAttendanceQrService
{
	Task<AttendanceQrResponse> GetQrCodeAsync(
		Guid eventId,
		AttendanceAction action,
		CancellationToken cancellationToken = default);

	Task<ValidateAttendanceQrResponse> ValidateQrCodeAsync(
		string token,
		CancellationToken cancellationToken = default);
}
