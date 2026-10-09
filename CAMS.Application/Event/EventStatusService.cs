using CAMS.Application.Common;
using CAMS.Application.Common.Clocking;

namespace CAMS.Application.Event;

public sealed class EventStatusService : IEventStatusService
{
	private readonly IEventRepository _eventRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IApplicationClock _clock;


	public EventStatusService(
		IEventRepository eventRepository,
		IUnitOfWork unitOfWork,
		IApplicationClock clock)
	{
		_eventRepository =
			eventRepository;

		_unitOfWork =
			unitOfWork;

		_clock =
			clock;
	}


	public async Task UpdateStatusesAsync(
		CancellationToken cancellationToken = default)
	{
		var localNow =
			_clock.LocalNow;


		var currentDate =
			DateOnly.FromDateTime(
				localNow);


		var events = await _eventRepository.GetByDateRangeAsync(
			startDate:
				currentDate.AddDays(-7),
			endDate:
				currentDate.AddDays(7),
			cancellationToken:
				cancellationToken);


		var hasChanges =
			false;


		foreach (
			var eventEntity in events)
		{
			var status =
				EventStatusResolver.Resolve(
					eventEntity.EventDate,
					eventEntity.StartTime,
					eventEntity.EndTime,
					localNow);


			if (
				eventEntity.Status ==
				status
			)
			{
				continue;
			}


			eventEntity.Status =
				status;

			eventEntity.UpdatedAt =
				_clock.UtcNow;


			hasChanges =
				true;
		}


		if (!hasChanges)
		{
			return;
		}


		await _unitOfWork.SaveChangesAsync(
			cancellationToken);
	}
}
