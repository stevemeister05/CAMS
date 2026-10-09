using CAMS.Application.Common.Exceptions;
using CAMS.Domain.Enums;

namespace CAMS.Application.Attendance;

public sealed class AttendanceWindowService
	: IAttendanceWindowService
{
	public bool IsOpen(
		Domain.Entities.Event @event,
		AttendanceAction action,
		DateTime localNow)
	{
		return GetState(
			@event,
			action,
			localNow) ==
			AttendanceWindowState.Open;
	}


	public void Validate(
		Domain.Entities.Event @event,
		AttendanceAction action,
		DateTime localNow)
	{
		var state =
			GetState(
				@event,
				action,
				localNow);


		if (
			state ==
			AttendanceWindowState.Open
		)
		{
			return;
		}


		if (
			action ==
			AttendanceAction.TimeIn
		)
		{
			if (
				state ==
				AttendanceWindowState.NotStarted
			)
			{
				throw new ConflictException(
					"The time-in attendance period " +
					"has not started yet.");
			}


			throw new ConflictException(
				"The time-in attendance period " +
				"has been closed.");
		}


		if (
			action ==
			AttendanceAction.TimeOut
		)
		{
			if (
				state ==
				AttendanceWindowState.NotStarted
			)
			{
				throw new ConflictException(
					"The time-out attendance period " +
					"has not started yet.");
			}


			throw new ConflictException(
				"The time-out attendance period " +
				"has been closed.");
		}


		throw new ValidationException(
			"Invalid attendance action.");
	}


	private static AttendanceWindowState GetState(
		Domain.Entities.Event @event,
		AttendanceAction action,
		DateTime localNow)
	{
		if (!Enum.IsDefined(action))
		{
			throw new ValidationException(
				"Invalid attendance action.");
		}


		var currentDate =
			DateOnly.FromDateTime(
				localNow);


		if (
			@event.EventDate >
			currentDate
		)
		{
			return AttendanceWindowState.NotStarted;
		}


		if (
			@event.EventDate <
			currentDate
		)
		{
			return AttendanceWindowState.Closed;
		}


		var currentTime =
			TimeOnly.FromDateTime(
				localNow);


		if (
			action ==
			AttendanceAction.TimeIn
		)
		{
			if (
				currentTime <
				@event.AttendanceTimeInStart
			)
			{
				return AttendanceWindowState.NotStarted;
			}


			if (
				currentTime >
				@event.AttendanceTimeInEnd
			)
			{
				return AttendanceWindowState.Closed;
			}


			return AttendanceWindowState.Open;
		}


		if (
			action ==
			AttendanceAction.TimeOut
		)
		{
			if (
				currentTime <
				@event.AttendanceTimeOutStart
			)
			{
				return AttendanceWindowState.NotStarted;
			}


			if (
				currentTime >
				@event.AttendanceTimeOutEnd
			)
			{
				return AttendanceWindowState.Closed;
			}


			return AttendanceWindowState.Open;
		}


		throw new ValidationException(
			"Invalid attendance action.");
	}


	private enum AttendanceWindowState
	{
		NotStarted = 1,
		Open = 2,
		Closed = 3
	}
}