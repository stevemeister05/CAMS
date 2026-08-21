using CAMS.Application.Common;
using CAMS.Application.Common.Exceptions;
using CAMS.Application.Common.Pagination;
using CAMS.Application.Event.DTOs;
using CAMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Application.Event;

public class EventService : IEventService
{
	private readonly IEventRepository _eventRepository;
	private readonly IUnitOfWork _unitOfWork;

	public EventService(
		IEventRepository eventRepository,
		IUnitOfWork unitOfWork)
	{
		_eventRepository = eventRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<EventResponse> GetByIdAsync(
		Guid id,
		CancellationToken cancellationToken = default)
	{
		var @event =
			await _eventRepository.GetByIdAsync(
				id,
				cancellationToken);

		if (@event is null)
		{
			throw new NotFoundException(
				"Event was not found.");
		}

		return MapToResponse(@event);
	}

	public async Task<PagedResult<EventResponse>> SearchAsync(
		PagedRequest<EventFilter> request,
		CancellationToken cancellationToken = default)
	{
		var result =
			await _eventRepository.SearchAsync(
				request,
				cancellationToken);

		return new PagedResult<EventResponse>(
			result.Items
				.Select(MapToResponse)
				.ToList(),
			result.TotalCount,
			result.Page,
			result.PageSize);
	}

	public async Task<EventResponse> CreateAsync(
		CreateEventRequest request,
		CancellationToken cancellationToken = default)
	{
		ValidateRequest(
			request.Name,
			request.Type,
			request.EventDate,
			request.StartTime,
			request.EndTime,
			request.AttendanceTimeInStart,
			request.AttendanceTimeInEnd,
			request.AttendanceTimeOutStart,
			request.AttendanceTimeOutEnd);

		var name = request.Name.Trim();

		var exists =
			await _eventRepository.ExistsAsync(
				request.Type,
				request.EventDate,
				request.StartTime,
				cancellationToken);

		if (exists)
		{
			throw new ConflictException(
				"An event with the same type, date, and start time already exists.");
		}

		var @event = new Domain.Entities.Event
		{
			Name = name,

			Type = request.Type,

			EventDate = request.EventDate,

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

			Status = request.Status,

			Description =
				string.IsNullOrWhiteSpace(request.Description)
					? null
					: request.Description.Trim(),

			CreatedAt = DateTime.UtcNow
		};

		await _eventRepository.AddAsync(
			@event,
			cancellationToken);

		await _unitOfWork.SaveChangesAsync(
			cancellationToken);

		return MapToResponse(@event);
	}

	public async Task<EventResponse> UpdateAsync(
		Guid id,
		UpdateEventRequest request,
		CancellationToken cancellationToken = default)
	{
		ValidateRequest(
			request.Name,
			request.Type,
			request.EventDate,
			request.StartTime,
			request.EndTime,
			request.AttendanceTimeInStart,
			request.AttendanceTimeInEnd,
			request.AttendanceTimeOutStart,
			request.AttendanceTimeOutEnd);

		var @event =
			await _eventRepository.GetByIdAsync(
				id,
				cancellationToken);

		if (@event is null)
		{
			throw new NotFoundException(
				"Event was not found.");
		}

		var exists =
			await _eventRepository.ExistsAsync(
				request.Type,
				request.EventDate,
				request.StartTime,
				cancellationToken);

		if (exists &&
			(
				@event.Type != request.Type ||
				@event.EventDate != request.EventDate ||
				@event.StartTime != request.StartTime
			))
		{
			throw new ConflictException(
				"An event with the same type, date, and start time already exists.");
		}

		@event.Name = request.Name.Trim();

		@event.Type = request.Type;

		@event.EventDate = request.EventDate;

		@event.StartTime = request.StartTime;
		@event.EndTime = request.EndTime;

		@event.AttendanceTimeInStart =
			request.AttendanceTimeInStart;

		@event.AttendanceTimeInEnd =
			request.AttendanceTimeInEnd;

		@event.AttendanceTimeOutStart =
			request.AttendanceTimeOutStart;

		@event.AttendanceTimeOutEnd =
			request.AttendanceTimeOutEnd;

		@event.Status = request.Status;

		@event.Description =
			string.IsNullOrWhiteSpace(request.Description)
				? null
				: request.Description.Trim();

		@event.UpdatedAt = DateTime.UtcNow;

		_eventRepository.Update(@event);

		await _unitOfWork.SaveChangesAsync(
			cancellationToken);

		return MapToResponse(@event);
	}

	public async Task DeleteAsync(
		Guid id,
		CancellationToken cancellationToken = default)
	{
		var @event =
			await _eventRepository.GetByIdAsync(
				id,
				cancellationToken);

		if (@event is null)
		{
			throw new NotFoundException(
				"Event was not found.");
		}

		_eventRepository.Delete(@event);

		await _unitOfWork.SaveChangesAsync(
			cancellationToken);
	}

	private static void ValidateRequest(
		string name,
		EventType type,
		DateOnly eventDate,
		TimeOnly startTime,
		TimeOnly endTime,
		TimeOnly attendanceTimeInStart,
		TimeOnly attendanceTimeInEnd,
		TimeOnly attendanceTimeOutStart,
		TimeOnly attendanceTimeOutEnd)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ValidationException(
				"Event name is required.");
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

		if (!Enum.IsDefined(type))
		{
			throw new ValidationException(
				"Invalid event type.");
		}
	}

	private static EventResponse MapToResponse(
		Domain.Entities.Event @event)
	{
		return new EventResponse
		{
			Id = @event.Id,

			Name = @event.Name,

			Type = @event.Type,

			EventDate = @event.EventDate,

			StartTime = @event.StartTime,
			EndTime = @event.EndTime,

			AttendanceTimeInStart =
				@event.AttendanceTimeInStart,

			AttendanceTimeInEnd =
				@event.AttendanceTimeInEnd,

			AttendanceTimeOutStart =
				@event.AttendanceTimeOutStart,

			AttendanceTimeOutEnd =
				@event.AttendanceTimeOutEnd,

			Status = @event.Status,

			Description = @event.Description,

			CreatedAt = @event.CreatedAt,
			UpdatedAt = @event.UpdatedAt
		};
	}
}
