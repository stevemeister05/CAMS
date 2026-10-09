using CAMS.Domain.Entities;
using CAMS.Domain.Enums;

namespace CAMS.Application.Attendance;

public interface IAttendanceQrSessionRepository
{
	Task<AttendanceQrSession?> GetByTokenAsync(
		string token,
		CancellationToken cancellationToken = default);

	Task<AttendanceQrSession?> GetByEventAndActionAsync(
		Guid eventId,
		AttendanceAction action,
		CancellationToken cancellationToken = default);

	Task AddAsync(
		AttendanceQrSession session,
		CancellationToken cancellationToken = default);

	void Delete(
		AttendanceQrSession session);
}
