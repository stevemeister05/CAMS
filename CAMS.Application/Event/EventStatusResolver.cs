using CAMS.Domain.Enums;

namespace CAMS.Application.Event;

public static class EventStatusResolver
{
	public static EventStatus Resolve(
		DateOnly eventDate,
		TimeOnly startTime,
		TimeOnly endTime,
		DateTime localNow)
	{
		var currentDate =
			DateOnly.FromDateTime(
				localNow);

		var currentTime =
			TimeOnly.FromDateTime(
				localNow);


		if (
			eventDate >
			currentDate
		)
		{
			return EventStatus.Scheduled;
		}


		if (
			eventDate <
			currentDate
		)
		{
			return EventStatus.Completed;
		}


		/*
		 * At this point, the event is today.
		 */

		if (
			currentTime <
			startTime
		)
		{
			return EventStatus.Scheduled;
		}


		if (
			currentTime <=
			endTime
		)
		{
			return EventStatus.Ongoing;
		}


		return EventStatus.Completed;
	}
}