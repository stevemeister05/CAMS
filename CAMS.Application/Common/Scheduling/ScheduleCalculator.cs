using CAMS.Application.Settings;
using CAMS.Domain.Enums;

namespace CAMS.Application.Common.Scheduling;

/// <summary>
/// Provides calculations for determining the next occurrence
/// of a configured schedule.
/// </summary>
public static class ScheduleCalculator
{
	/// <summary>
	/// Gets the next occurrence of the specified schedule after
	/// the supplied date and time.
	/// </summary>
	/// <param name="schedule">
	/// The schedule configuration.
	/// </param>
	/// <param name="from">
	/// The date and time from which the next occurrence should be calculated.
	/// </param>
	/// <returns>
	/// The next occurrence, or null if no valid occurrence can be calculated.
	/// </returns>
	public static DateTime? GetNextOccurrence(
		ScheduleSettings schedule,
		DateTime from)
	{
		ArgumentNullException.ThrowIfNull(schedule);

		Validate(schedule);

		return schedule.Type switch
		{
			ScheduleType.Yearly =>
				GetNextYearly(schedule, from),

			ScheduleType.Monthly =>
				GetNextMonthly(schedule, from),

			ScheduleType.Weekly =>
				GetNextWeekly(schedule, from),

			ScheduleType.Daily =>
				GetNextDaily(schedule, from),

			ScheduleType.Hourly =>
				GetNextHourly(schedule, from),

			ScheduleType.Minutely =>
				GetNextMinutely(schedule, from),

			ScheduleType.Interval =>
				GetNextInterval(schedule, from),

			_ => throw new InvalidOperationException(
				$"Unsupported schedule type: {schedule.Type}.")
		};
	}

	private static DateTime GetNextYearly(
		ScheduleSettings schedule,
		DateTime from)
	{
		var month = schedule.Month!.Value;
		var day = schedule.Day!.Value;
		var hour = schedule.Hour!.Value;
		var minute = schedule.Minute!.Value;
		var second = schedule.Second!.Value;

		for (var year = from.Year; ; year++)
		{
			if (day >
				DateTime.DaysInMonth(year, month))
			{
				continue;
			}

			var occurrence = new DateTime(
				year,
				month,
				day,
				hour,
				minute,
				second,
				from.Kind);

			if (occurrence > from)
			{
				return occurrence;
			}
		}
	}

	private static DateTime GetNextMonthly(
		ScheduleSettings schedule,
		DateTime from)
	{
		var day = schedule.Day!.Value;
		var hour = schedule.Hour!.Value;
		var minute = schedule.Minute!.Value;
		var second = schedule.Second!.Value;

		var year = from.Year;
		var month = from.Month;

		for (var i = 0; i < 120; i++)
		{
			if (day <= DateTime.DaysInMonth(year, month))
			{
				var occurrence = new DateTime(
					year,
					month,
					day,
					hour,
					minute,
					second,
					from.Kind);

				if (occurrence > from)
				{
					return occurrence;
				}
			}

			month++;

			if (month > 12)
			{
				month = 1;
				year++;
			}
		}

		throw new InvalidOperationException(
			"Unable to calculate the next monthly schedule occurrence.");
	}

	private static DateTime GetNextWeekly(
		ScheduleSettings schedule,
		DateTime from)
	{
		var targetDay = schedule.DayOfWeek!.Value;

		var hour = schedule.Hour!.Value;
		var minute = schedule.Minute!.Value;
		var second = schedule.Second!.Value;

		var daysUntilTarget =
			((int)targetDay - (int)from.DayOfWeek + 7) % 7;

		var occurrenceDate =
			from.Date.AddDays(daysUntilTarget);

		var occurrence = occurrenceDate
			.AddHours(hour)
			.AddMinutes(minute)
			.AddSeconds(second);

		if (occurrence <= from)
		{
			occurrence = occurrence.AddDays(7);
		}

		return occurrence;
	}

	private static DateTime GetNextDaily(
		ScheduleSettings schedule,
		DateTime from)
	{
		var occurrence = from.Date
			.AddHours(schedule.Hour!.Value)
			.AddMinutes(schedule.Minute!.Value)
			.AddSeconds(schedule.Second!.Value);

		if (occurrence <= from)
		{
			occurrence = occurrence.AddDays(1);
		}

		return occurrence;
	}

	private static DateTime GetNextHourly(
		ScheduleSettings schedule,
		DateTime from)
	{
		var occurrence = new DateTime(
			from.Year,
			from.Month,
			from.Day,
			from.Hour,
			schedule.Minute!.Value,
			schedule.Second!.Value,
			from.Kind);

		if (occurrence <= from)
		{
			occurrence = occurrence.AddHours(1);
		}

		return occurrence;
	}

	private static DateTime GetNextMinutely(
		ScheduleSettings schedule,
		DateTime from)
	{
		var occurrence = new DateTime(
			from.Year,
			from.Month,
			from.Day,
			from.Hour,
			from.Minute,
			schedule.Second!.Value,
			from.Kind);

		if (occurrence <= from)
		{
			occurrence = occurrence.AddMinutes(1);
		}

		return occurrence;
	}

	private static DateTime GetNextInterval(
		ScheduleSettings schedule,
		DateTime from)
	{
		return from.AddSeconds(
			schedule.IntervalSeconds!.Value);
	}

	private static void Validate(
		ScheduleSettings schedule)
	{
		switch (schedule.Type)
		{
			case ScheduleType.Yearly:

				Require(
					schedule.Month,
					"Month");

				Require(
					schedule.Day,
					"Day");

				Require(
					schedule.Hour,
					"Hour");

				Require(
					schedule.Minute,
					"Minute");

				Require(
					schedule.Second,
					"Second");

				ValidateRange(
					schedule.Month.Value,
					1,
					12,
					"Month");

				ValidateRange(
					schedule.Day.Value,
					1,
					31,
					"Day");

				ValidateTime(
					schedule.Hour.Value,
					schedule.Minute.Value,
					schedule.Second.Value);

				break;

			case ScheduleType.Monthly:

				Require(
					schedule.Day,
					"Day");

				Require(
					schedule.Hour,
					"Hour");

				Require(
					schedule.Minute,
					"Minute");

				Require(
					schedule.Second,
					"Second");

				ValidateRange(
					schedule.Day.Value,
					1,
					31,
					"Day");

				ValidateTime(
					schedule.Hour.Value,
					schedule.Minute.Value,
					schedule.Second.Value);

				break;

			case ScheduleType.Weekly:

				if (schedule.DayOfWeek is null)
				{
					throw new ArgumentException(
						"DayOfWeek is required for weekly schedules.");
				}

				Require(
					schedule.Hour,
					"Hour");

				Require(
					schedule.Minute,
					"Minute");

				Require(
					schedule.Second,
					"Second");

				ValidateTime(
					schedule.Hour.Value,
					schedule.Minute.Value,
					schedule.Second.Value);

				break;

			case ScheduleType.Daily:

				Require(
					schedule.Hour,
					"Hour");

				Require(
					schedule.Minute,
					"Minute");

				Require(
					schedule.Second,
					"Second");

				ValidateTime(
					schedule.Hour.Value,
					schedule.Minute.Value,
					schedule.Second.Value);

				break;

			case ScheduleType.Hourly:

				Require(
					schedule.Minute,
					"Minute");

				Require(
					schedule.Second,
					"Second");

				ValidateRange(
					schedule.Minute.Value,
					0,
					59,
					"Minute");

				ValidateRange(
					schedule.Second.Value,
					0,
					59,
					"Second");

				break;

			case ScheduleType.Minutely:

				Require(
					schedule.Second,
					"Second");

				ValidateRange(
					schedule.Second.Value,
					0,
					59,
					"Second");

				break;

			case ScheduleType.Interval:

				if (!schedule.IntervalSeconds.HasValue ||
					schedule.IntervalSeconds.Value <= 0)
				{
					throw new ArgumentException(
						"IntervalSeconds must be greater than zero.");
				}

				break;

			default:

				throw new ArgumentException(
					$"Unsupported schedule type: {schedule.Type}.");
		}
	}

	private static void Require(
		int? value,
		string propertyName)
	{
		if (!value.HasValue)
		{
			throw new ArgumentException(
				$"{propertyName} is required.");
		}
	}

	private static void ValidateRange(
		int value,
		int minimum,
		int maximum,
		string propertyName)
	{
		if (value < minimum ||
			value > maximum)
		{
			throw new ArgumentOutOfRangeException(
				propertyName,
				value,
				$"{propertyName} must be between {minimum} and {maximum}.");
		}
	}

	private static void ValidateTime(
		int hour,
		int minute,
		int second)
	{
		ValidateRange(
			hour,
			0,
			23,
			"Hour");

		ValidateRange(
			minute,
			0,
			59,
			"Minute");

		ValidateRange(
			second,
			0,
			59,
			"Second");
	}
}