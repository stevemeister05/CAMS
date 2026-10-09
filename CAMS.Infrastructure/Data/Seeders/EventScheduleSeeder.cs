using CAMS.Domain.Entities;
using CAMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Infrastructure.Data.Seeders;

public static class EventScheduleSeeder
{
	public static async Task SeedAsync(
		CAMSDBContext dbContext,
		CancellationToken cancellationToken = default)
	{
		var schedules = new[]
		{
			new EventSchedule
			{
				Id = Guid.Parse(
					"11111111-1111-1111-1111-111111111111"),

				Name = "Sunday Mass - 7:30 AM",

				EventType = EventType.SundayMass,

				IsActive = true,

				StartTime = new TimeOnly(7, 30),
				EndTime = new TimeOnly(8, 30),

				AttendanceTimeInStart =
					new TimeOnly(6, 30),

				AttendanceTimeInEnd =
					new TimeOnly(7, 45),

				AttendanceTimeOutStart =
					new TimeOnly(8, 15),

				AttendanceTimeOutEnd =
					new TimeOnly(9, 30),

				DayOfWeek = DayOfWeek.Sunday,

				StartMonth = null,
				StartDay = null,

				EndMonth = null,
				EndDay = null
			},

			new EventSchedule
			{
				Id = Guid.Parse(
					"22222222-2222-2222-2222-222222222222"),

				Name = "Sunday Mass - 5:00 PM",

				EventType = EventType.SundayMass,

				IsActive = true,

				StartTime = new TimeOnly(17, 0),
				EndTime = new TimeOnly(18, 0),

				AttendanceTimeInStart =
					new TimeOnly(16, 0),

				AttendanceTimeInEnd =
					new TimeOnly(17, 15),

				AttendanceTimeOutStart =
					new TimeOnly(17, 45),

				AttendanceTimeOutEnd =
					new TimeOnly(19, 0),

				DayOfWeek = DayOfWeek.Sunday,

				StartMonth = null,
				StartDay = null,

				EndMonth = null,
				EndDay = null
			},

			new EventSchedule
			{
				Id = Guid.Parse(
					"33333333-3333-3333-3333-333333333333"),

				Name = "Misa de Gallo - 4:00 AM",

				EventType = EventType.MisaDeGallo,

				IsActive = true,

				StartTime = new TimeOnly(4, 0),
				EndTime = new TimeOnly(5, 0),

				AttendanceTimeInStart =
					new TimeOnly(3, 0),

				AttendanceTimeInEnd =
					new TimeOnly(4, 15),

				AttendanceTimeOutStart =
					new TimeOnly(4, 45),

				AttendanceTimeOutEnd =
					new TimeOnly(6, 0),

				DayOfWeek = null,

				StartMonth = 12,
				StartDay = 16,

				EndMonth = 12,
				EndDay = 24
			}
		};

		foreach (var schedule in schedules)
		{
			var exists = await dbContext.EventSchedules
				.AnyAsync(
					x => x.Id == schedule.Id,
					cancellationToken);

			if (exists)
			{
				continue;
			}

			schedule.CreatedAt = DateTime.UtcNow;

			await dbContext.EventSchedules.AddAsync(
				schedule,
				cancellationToken);
		}

		await dbContext.SaveChangesAsync(
			cancellationToken);
	}
}
