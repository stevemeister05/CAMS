using CAMS.Application.Common;
using CAMS.Application.EventSchedule;
using CAMS.Application.Settings;
using CAMS.Domain.Enums;
using Microsoft.Extensions.Options;

namespace CAMS.Application.Event;

public class EventGenerationService : IEventGenerationService
{
	private readonly IEventScheduleRepository _eventScheduleRepository;
	private readonly IEventRepository _eventRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly EventGenerationSettings _settings;

	public EventGenerationService(
		IEventScheduleRepository eventScheduleRepository,
		IEventRepository eventRepository,
		IUnitOfWork unitOfWork,
		IOptions<EventGenerationSettings> options)
	{
		_eventScheduleRepository = eventScheduleRepository;
		_eventRepository = eventRepository;
		_unitOfWork = unitOfWork;
		_settings = options.Value;
	}

	public async Task GenerateUpcomingEventsAsync(
		CancellationToken cancellationToken = default)
	{
		if (_settings.GenerateDaysAhead <= 0)
		{
			throw new InvalidOperationException(
				"EventGeneration:GenerateDaysAhead must be greater than zero.");
		}

		var today = DateOnly.FromDateTime(
			DateTime.Now);

		var endDate = today.AddDays(
			_settings.GenerateDaysAhead);

		var schedules =
			await _eventScheduleRepository.GetActiveSchedulesAsync(
				cancellationToken);

		if (schedules.Count == 0)
		{
			return;
		}

		var existingEvents =
			await _eventRepository.GetByDateRangeAsync(
				today,
				endDate,
				cancellationToken);

		var existingEventKeys = existingEvents
			.Select(x => new EventKey(
				x.Type,
				x.EventDate,
				x.StartTime))
			.ToHashSet();

		var eventsToCreate =
			new List<Domain.Entities.Event>();

		foreach (var schedule in schedules)
		{
			switch (schedule.EventType)
			{
				case EventType.SundayMass:

					GenerateSundayMassEvents(
						schedule,
						today,
						endDate,
						existingEventKeys,
						eventsToCreate);

					break;

				case EventType.MisaDeGallo:

					GenerateMisaDeGalloEvents(
						schedule,
						today,
						endDate,
						existingEventKeys,
						eventsToCreate);

					break;

				case EventType.SpecialEvent:

					// Special events are created manually.
					break;
			}
		}

		if (eventsToCreate.Count == 0)
		{
			return;
		}

		await _eventRepository.AddRangeAsync(
			eventsToCreate,
			cancellationToken);

		await _unitOfWork.SaveChangesAsync(
			cancellationToken);
	}

	public async Task RegenerateFutureEventsAsync(
	CancellationToken cancellationToken = default)
	{
		var today = DateOnly.FromDateTime(
			DateTime.Now);

		await using var transaction =
			await _unitOfWork.BeginTransactionAsync(
				cancellationToken);

		try
		{
			var futureEvents =
				await _eventRepository.GetByDateRangeAsync(
					startDate: today.AddDays(1),
					cancellationToken: cancellationToken);

			var eventsToDelete = futureEvents
				.Where(x =>
					x.Type != EventType.SpecialEvent)
				.ToList();

			if (eventsToDelete.Count > 0)
			{
				_eventRepository.DeleteRange(
					eventsToDelete);

				await _unitOfWork.SaveChangesAsync(
					cancellationToken);
			}

			await GenerateUpcomingEventsAsync(
				cancellationToken);

			await transaction.CommitAsync(
				cancellationToken);
		}
		catch
		{
			await transaction.RollbackAsync(
				cancellationToken);

			throw;
		}
	}

	private static void GenerateSundayMassEvents(
		Domain.Entities.EventSchedule schedule,
		DateOnly startDate,
		DateOnly endDate,
		HashSet<EventKey> existingEventKeys,
		List<Domain.Entities.Event> eventsToCreate)
	{
		if (schedule.DayOfWeek is null)
		{
			return;
		}

		var currentDate = startDate;

		while (currentDate <= endDate)
		{
			if (currentDate.DayOfWeek ==
				schedule.DayOfWeek.Value)
			{
				AddEventIfMissing(
					schedule,
					currentDate,
					existingEventKeys,
					eventsToCreate);
			}

			currentDate = currentDate.AddDays(1);
		}
	}

	private static void GenerateMisaDeGalloEvents(
		Domain.Entities.EventSchedule schedule,
		DateOnly startDate,
		DateOnly endDate,
		HashSet<EventKey> existingEventKeys,
		List<Domain.Entities.Event> eventsToCreate)
	{
		if (!HasSeasonalRange(schedule))
		{
			return;
		}

		var currentDate = startDate;

		while (currentDate <= endDate)
		{
			if (IsDateWithinSeason(
				currentDate,
				schedule))
			{
				AddEventIfMissing(
					schedule,
					currentDate,
					existingEventKeys,
					eventsToCreate);
			}

			currentDate = currentDate.AddDays(1);
		}
	}

	private static bool HasSeasonalRange(
		Domain.Entities.EventSchedule schedule)
	{
		return schedule.StartMonth.HasValue &&
			   schedule.StartDay.HasValue &&
			   schedule.EndMonth.HasValue &&
			   schedule.EndDay.HasValue;
	}

	private static bool IsDateWithinSeason(
		DateOnly date,
		Domain.Entities.EventSchedule schedule)
	{
		if (!HasSeasonalRange(schedule))
		{
			return false;
		}

		var current =
			date.Month * 100 +
			date.Day;

		var start =
			schedule.StartMonth!.Value * 100 +
			schedule.StartDay!.Value;

		var end =
			schedule.EndMonth!.Value * 100 +
			schedule.EndDay!.Value;

		// Normal range.
		//
		// Example:
		// December 16 -> December 24
		//
		// 1216 <= current <= 1224
		if (start <= end)
		{
			return current >= start &&
				   current <= end;
		}

		// Range crosses the end of the year.
		//
		// Example:
		// December 16 -> January 5
		//
		// December 20:
		// 1220 >= 1216 -> true
		//
		// January 3:
		// 103 <= 105 -> true
		return current >= start ||
			   current <= end;
	}

	private static void AddEventIfMissing(
		Domain.Entities.EventSchedule schedule,
		DateOnly eventDate,
		HashSet<EventKey> existingEventKeys,
		List<Domain.Entities.Event> eventsToCreate)
	{
		var key = new EventKey(
			schedule.EventType,
			eventDate,
			schedule.StartTime);

		if (existingEventKeys.Contains(key))
		{
			return;
		}

		var @event = new Domain.Entities.Event
		{
			Name = schedule.Name,

			Type = schedule.EventType,

			EventDate = eventDate,

			StartTime = schedule.StartTime,
			EndTime = schedule.EndTime,

			AttendanceTimeInStart =
				schedule.AttendanceTimeInStart,

			AttendanceTimeInEnd =
				schedule.AttendanceTimeInEnd,

			AttendanceTimeOutStart =
				schedule.AttendanceTimeOutStart,

			AttendanceTimeOutEnd =
				schedule.AttendanceTimeOutEnd,

			Status = EventStatus.Scheduled,

			CreatedAt = DateTime.UtcNow
		};

		eventsToCreate.Add(@event);

		// Add immediately so another schedule cannot
		// generate the same event during this run.
		existingEventKeys.Add(key);
	}

	private readonly record struct EventKey(
		EventType Type,
		DateOnly EventDate,
		TimeOnly StartTime);
}
