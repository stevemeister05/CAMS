using System.Collections.Concurrent;
using CAMS.Domain.Enums;

namespace CAMS.Web.Realtime;

public sealed class AttendanceQrSubscriptionRegistry
	: IAttendanceQrSubscriptionRegistry
{
	private readonly ConcurrentDictionary<
		string,
		AttendanceQrSubscription>
		_subscriptions = new();

	public void Set(
		string connectionId,
		Guid eventId,
		AttendanceAction action,
		DateTime expiresAt)
	{
		var subscription =
			new AttendanceQrSubscription(
				connectionId,
				eventId,
				action,
				expiresAt);

		_subscriptions.AddOrUpdate(
			connectionId,
			subscription,
			(_, _) => subscription);
	}

	public void Remove(
		string connectionId)
	{
		_subscriptions.TryRemove(
			connectionId,
			out _);
	}

	public IReadOnlyList<AttendanceQrTarget>
		GetExpiredTargets(
			DateTime utcNow)
	{
		return _subscriptions
			.Values
			.Where(x =>
				x.ExpiresAt <= utcNow)
			.Select(x =>
				new AttendanceQrTarget(
					x.EventId,
					x.Action))
			.Distinct()
			.ToList();
	}

	public void UpdateExpiration(
		Guid eventId,
		AttendanceAction action,
		DateTime expiresAt)
	{
		foreach (var pair in _subscriptions)
		{
			var current =
				pair.Value;

			if (current.EventId != eventId ||
				current.Action != action)
			{
				continue;
			}

			var updated =
				current with
				{
					ExpiresAt = expiresAt
				};

			_subscriptions.TryUpdate(
				pair.Key,
				updated,
				current);
		}
	}

	private sealed record AttendanceQrSubscription(
		string ConnectionId,
		Guid EventId,
		AttendanceAction Action,
		DateTime ExpiresAt);
}