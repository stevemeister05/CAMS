using CAMS.Application.Common;
using CAMS.Application.Common.Clocking;
using CAMS.Application.Common.Exceptions;
using CAMS.Application.Common.Pagination;
using CAMS.Application.EventSchedule.DTOs;
using CAMS.Domain.Enums;

namespace CAMS.Application.EventSchedule;

public class EventScheduleService : IEventScheduleService
{
	private readonly IEventScheduleRepository _eventScheduleRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IApplicationClock _clock;

	public EventScheduleService(
		IEventScheduleRepository eventScheduleRepository,
		IUnitOfWork unitOfWork,
		IApplicationClock clock)
	{
		_eventScheduleRepository = eventScheduleRepository;
		_unitOfWork = unitOfWork;
		_clock = clock;
	}

	public async Task<EventScheduleResponse> GetByIdAsync(
		Guid id,
		CancellationToken cancellationToken = default)
	{
		var schedule =
			await _eventScheduleRepository.GetByIdAsync(
				id,
				cancellationToken);

		if (schedule is null)
		{
			throw new NotFoundException(
				"Event schedule was not found.");
		}

		return MapToResponse(schedule);
	}

	public async Task<PagedResult<EventScheduleResponse>> SearchAsync(
		PagedRequest<EventScheduleFilter> request,
		CancellationToken cancellationToken = default)
	{
		var result =
			await _eventScheduleRepository.SearchAsync(
				request,
				cancellationToken);

		return new PagedResult<EventScheduleResponse>(
			result.Items
				.Select(MapToResponse)
				.ToList(),
			result.TotalCount,
			result.Page,
			result.PageSize);
	}

	public async Task<EventScheduleResponse> CreateAsync(
		CreateEventScheduleRequest request,
		CancellationToken cancellationToken = default)
	{
		ValidateRequest(
			request.Name,
			request.EventType,
			request.StartTime,
			request.EndTime,
			request.AttendanceTimeInStart,
			request.AttendanceTimeInEnd,
			request.AttendanceTimeOutStart,
			request.AttendanceTimeOutEnd,
			request.DayOfWeek,
			request.StartMonth,
			request.StartDay,
			request.EndMonth,
			request.EndDay);

		var name = request.Name.Trim();

		var existing =
			await _eventScheduleRepository.GetByNameAsync(
				name,
				cancellationToken);

		if (existing is not null)
		{
			throw new ConflictException(
				"An event schedule with this name already exists.");
		}

		var schedule = new Domain.Entities.EventSchedule
		{
			Name = name,

			EventType = request.EventType,

			IsActive = request.IsActive,

			StartTime = request.StartTime,
			EndTime = request.EndTime,

			AttendanceTimeInStart =
				request.AttendanceTimeInStart,

			AttendanceTimeInEnd =
				request.AttendanceTimeInEnd,

			AttendanceTimeOutStart =
				request.AttendanceTimeOutStart,

			AttendanceTimeOutEnd =
				request.AttendanceTimeOutEnd,

			DayOfWeek = request.DayOfWeek,

			StartMonth = request.StartMonth,
			StartDay = request.StartDay,

			EndMonth = request.EndMonth,
			EndDay = request.EndDay,

			CreatedAt = _clock.UtcNow
		};

		await _eventScheduleRepository.AddAsync(
			schedule,
			cancellationToken);

		await _unitOfWork.SaveChangesAsync(
			cancellationToken);

		return MapToResponse(schedule);
	}

	public async Task<EventScheduleResponse> UpdateAsync(
		Guid id,
		UpdateEventScheduleRequest request,
		CancellationToken cancellationToken = default)
	{
		ValidateRequest(
			request.Name,
			request.EventType,
			request.StartTime,
			request.EndTime,
			request.AttendanceTimeInStart,
			request.AttendanceTimeInEnd,
			request.AttendanceTimeOutStart,
			request.AttendanceTimeOutEnd,
			request.DayOfWeek,
			request.StartMonth,
			request.StartDay,
			request.EndMonth,
			request.EndDay);

		var schedule =
			await _eventScheduleRepository.GetByIdAsync(
				id,
				cancellationToken);

		if (schedule is null)
		{
			throw new NotFoundException(
				"Event schedule was not found.");
		}

		var name = request.Name.Trim();

		var existing =
			await _eventScheduleRepository.GetByNameAsync(
				name,
				cancellationToken);

		if (existing is not null &&
			existing.Id != id)
		{
			throw new ConflictException(
				"An event schedule with this name already exists.");
		}

		schedule.Name = name;

		schedule.EventType = request.EventType;

		schedule.IsActive = request.IsActive;

		schedule.StartTime = request.StartTime;
		schedule.EndTime = request.EndTime;

		schedule.AttendanceTimeInStart =
			request.AttendanceTimeInStart;

		schedule.AttendanceTimeInEnd =
			request.AttendanceTimeInEnd;

		schedule.AttendanceTimeOutStart =
			request.AttendanceTimeOutStart;

		schedule.AttendanceTimeOutEnd =
			request.AttendanceTimeOutEnd;

		schedule.DayOfWeek = request.DayOfWeek;

		schedule.StartMonth = request.StartMonth;
		schedule.StartDay = request.StartDay;

		schedule.EndMonth = request.EndMonth;
		schedule.EndDay = request.EndDay;

		schedule.UpdatedAt = _clock.UtcNow;

		_eventScheduleRepository.Update(schedule);

		await _unitOfWork.SaveChangesAsync(
			cancellationToken);

		return MapToResponse(schedule);
	}

	public async Task DeleteAsync(
		Guid id,
		CancellationToken cancellationToken = default)
	{
		var schedule =
			await _eventScheduleRepository.GetByIdAsync(
				id,
				cancellationToken);

		if (schedule is null)
		{
			throw new NotFoundException(
				"Event schedule was not found.");
		}

		_eventScheduleRepository.Delete(schedule);

		await _unitOfWork.SaveChangesAsync(
			cancellationToken);
	}

	private static void ValidateRequest(
		string name,
		EventType eventType,
		TimeOnly startTime,
		TimeOnly endTime,
		TimeOnly attendanceTimeInStart,
		TimeOnly attendanceTimeInEnd,
		TimeOnly attendanceTimeOutStart,
		TimeOnly attendanceTimeOutEnd,
		DayOfWeek? dayOfWeek,
		int? startMonth,
		int? startDay,
		int? endMonth,
		int? endDay)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ValidationException(
				"Event schedule name is required.");
		}

		if (startTime >= endTime)
		{
			throw new ValidationException(
				"Event start time must be earlier than the end time.");
		}

		if (attendanceTimeInStart >= attendanceTimeInEnd)
		{
			throw new ValidationException(
				"Attendance time-in start must be earlier than the end.");
		}

		if (attendanceTimeOutStart >= attendanceTimeOutEnd)
		{
			throw new ValidationException(
				"Attendance time-out start must be earlier than the end.");
		}

		var hasStartDate =
			startMonth.HasValue ||
			startDay.HasValue;

		var hasEndDate =
			endMonth.HasValue ||
			endDay.HasValue;

		// Start and end seasonal dates must either
		// both be supplied or both be omitted.
		if (hasStartDate != hasEndDate)
		{
			throw new ValidationException(
				"Start and end seasonal dates must either both be provided or both be omitted.");
		}

		if (hasStartDate)
		{
			if (!startMonth.HasValue ||
				!startDay.HasValue ||
				!endMonth.HasValue ||
				!endDay.HasValue)
			{
				throw new ValidationException(
					"StartMonth, StartDay, EndMonth, and EndDay must all be provided.");
			}

			ValidateMonth(
				startMonth.Value,
				nameof(startMonth));

			ValidateMonth(
				endMonth.Value,
				nameof(endMonth));

			ValidateDay(
				startMonth.Value,
				startDay.Value,
				nameof(startDay));

			ValidateDay(
				endMonth.Value,
				endDay.Value,
				nameof(endDay));
		}

		switch (eventType)
		{
			case EventType.SundayMass:

				if (dayOfWeek != DayOfWeek.Sunday)
				{
					throw new ValidationException(
						"Sunday Mass schedules must be scheduled for Sunday.");
				}

				if (hasStartDate)
				{
					throw new ValidationException(
						"Sunday Mass schedules cannot have a seasonal date range.");
				}

				break;

			case EventType.MisaDeGallo:

				if (!hasStartDate)
				{
					throw new ValidationException(
						"Misa de Gallo schedules must have a start and end seasonal date.");
				}

				if (dayOfWeek is not null)
				{
					throw new ValidationException(
						"Misa de Gallo schedules cannot specify a day of the week.");
				}

				break;

			case EventType.SpecialEvent:

				throw new ValidationException(
					"There is no schedule configuration for special events.");

			default:

				throw new ValidationException(
					"Invalid event type.");
		}
	}

	private static void ValidateMonth(
		int month,
		string propertyName)
	{
		if (month < 1 || month > 12)
		{
			throw new ValidationException(
				$"{propertyName} must be between 1 and 12.");
		}
	}

	private static void ValidateDay(
		int month,
		int day,
		string propertyName)
	{
		// 2000 is a leap year, allowing February 29
		// to be configured as a recurring seasonal date.
		var daysInMonth =
			DateTime.DaysInMonth(
				2000,
				month);

		if (day < 1 || day > daysInMonth)
		{
			throw new ValidationException(
				$"{propertyName} is invalid for the specified month.");
		}
	}

	private static EventScheduleResponse MapToResponse(
		Domain.Entities.EventSchedule schedule)
	{
		return new EventScheduleResponse
		{
			Id = schedule.Id,

			Name = schedule.Name,

			EventType = schedule.EventType,

			IsActive = schedule.IsActive,

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

			DayOfWeek = schedule.DayOfWeek,

			StartMonth = schedule.StartMonth,
			StartDay = schedule.StartDay,

			EndMonth = schedule.EndMonth,
			EndDay = schedule.EndDay,

			CreatedAt = schedule.CreatedAt,
			UpdatedAt = schedule.UpdatedAt
		};
	}
}
