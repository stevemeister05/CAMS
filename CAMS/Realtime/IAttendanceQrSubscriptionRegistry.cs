using CAMS.Domain.Enums;

namespace CAMS.Web.Realtime;

public interface IAttendanceQrSubscriptionRegistry
{
	void Set(
		string connectionId,
		Guid eventId,
		AttendanceAction action,
		DateTime expiresAt);

	void Remove(
		string connectionId);

	IReadOnlyList<AttendanceQrTarget>
		GetExpiredTargets(
			DateTime utcNow);

	void UpdateExpiration(
		Guid eventId,
		AttendanceAction action,
		DateTime expiresAt);
}
