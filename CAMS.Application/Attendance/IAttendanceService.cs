using CAMS.Application.Attendance.DTOs;
using CAMS.Domain.Enums;

namespace CAMS.Application.Attendance;

public interface IAttendanceService
{
	Task<AttendancePageResponse> GetAttendanceByEventAsync(
		Guid eventId,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<MemberAttendanceHistoryResponse>> GetByMemberAsync(
		Guid memberId,
		CancellationToken cancellationToken = default);

	Task<AttendanceResponse> RecordQrAttendanceAsync(
		Guid memberId,
		RecordQrAttendanceRequest request,
		CancellationToken cancellationToken = default);

	Task<AttendanceResponse> RecordFingerprintAttendanceAsync(
		RecordFingerprintAttendanceRequest request,
		CancellationToken cancellationToken = default);

	Task<AttendanceResponse> RecordManualAttendanceAsync(
		RecordManualAttendanceRequest request,
		CancellationToken cancellationToken = default);
}
