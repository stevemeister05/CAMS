using CAMS.Application.Dashboard;
using CAMS.Application.Dashboard.DTOs;
using CAMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CAMS.Infrastructure.Repositories;

public sealed class DashboardRepository : IDashboardRepository
{
	private readonly CAMSDBContext _context;


	public DashboardRepository(
		CAMSDBContext context)
	{
		_context =
			context;
	}


	public async Task<AdminDashboardResponse> GetAdminDashboardAsync(
		DateOnly today,
		int trendDays,
		int recentAttendanceCount,
		int upcomingEventCount,
		CancellationToken cancellationToken = default)
	{
		var activeMemberCount =
			await _context.Members
				.AsNoTracking()
				.CountAsync(
					x => x.IsActive,
					cancellationToken);


		/*
		 * If you already have a RegistrationStatus.Pending
		 * enum, you can replace this predicate with Status.
		 *
		 * This works with your current RegistrationRequest
		 * ApprovedAt / RejectedAt fields.
		 */
		var pendingRegistrationCount =
			await _context.RegistrationRequests
				.AsNoTracking()
				.CountAsync(
					x =>
						x.ApprovedAt == null &&
						x.RejectedAt == null,
					cancellationToken);


		var todayAttendanceCount =
			await _context.Attendances
				.AsNoTracking()
				.Where(
					x =>
						x.Event.EventDate ==
						today)
				.Select(
					x =>
						x.MemberId)
				.Distinct()
				.CountAsync(
					cancellationToken);


		var todayEventsRaw =
			await _context.Events
				.AsNoTracking()
				.Where(
					x =>
						x.EventDate ==
						today)
				.OrderBy(
					x =>
						x.StartTime)
				.Select(
					x =>
						new
						{
							x.Id,
							x.Name,
							x.EventDate,
							x.StartTime,
							x.EndTime,
							x.AttendanceTimeInStart,
							x.AttendanceTimeInEnd,
							x.AttendanceTimeOutStart,
							x.AttendanceTimeOutEnd,
							x.Type,
							x.Status,
							AttendanceCount =
								x.Attendances
									.Select(
										a =>
											a.MemberId)
									.Distinct()
									.Count()
						})
				.ToListAsync(
					cancellationToken);


		var todayEvents =
			todayEventsRaw
				.Select(
					x =>
						new DashboardEventResponse
						{
							Id =
								x.Id,

							Name =
								x.Name,

							EventDate =
								x.EventDate,

							StartTime =
								x.StartTime,

							EndTime =
								x.EndTime,

							Type =
								x.Type.ToString(),

							Status =
								x.Status.ToString(),

							AttendanceCount =
								x.AttendanceCount,
							AttendanceTimeInStart = x.AttendanceTimeInStart,
							AttendanceTimeInEnd = x.AttendanceTimeInEnd,
							AttendanceTimeOutStart = x.AttendanceTimeOutStart,
							AttendanceTimeOutEnd = x.AttendanceTimeOutEnd
						})
				.ToList();


		var trendStartDate =
			today.AddDays(
				-(trendDays - 1));


		var trendRaw =
			await _context.Attendances
				.AsNoTracking()
				.Where(
					x =>
						x.Event.EventDate >=
							trendStartDate &&
						x.Event.EventDate <=
							today)
				.GroupBy(
					x =>
						x.Event.EventDate)
				.Select(
					x =>
						new
						{
							Date =
								x.Key,

							AttendanceCount =
								x.Select(
										a =>
											a.MemberId)
									.Distinct()
									.Count()
						})
				.ToListAsync(
					cancellationToken);


		var trendLookup =
			trendRaw.ToDictionary(
				x =>
					x.Date,

				x =>
					x.AttendanceCount);


		var attendanceTrend =
			new List<AttendanceTrendPointResponse>();


		for (
			var date = trendStartDate;
			date <= today;
			date = date.AddDays(1))
		{
			attendanceTrend.Add(
				new AttendanceTrendPointResponse
				{
					Date =
						date,

					AttendanceCount =
						trendLookup.GetValueOrDefault(
							date)
				});
		}


		var recentRaw =
			await _context.Attendances
				.AsNoTracking()
				.OrderByDescending(
					x =>
						x.TimeIn)
				.Take(
					recentAttendanceCount)
				.Select(
					x =>
						new
						{
							x.Id,
							x.MemberId,
							x.Member.FirstName,
							x.Member.MiddleName,
							x.Member.LastName,
							x.EventId,

							EventName =
								x.Event.Name,

							x.TimeIn,
							x.TimeOut
						})
				.ToListAsync(
					cancellationToken);


		var recentAttendances =
			recentRaw
				.Select(
					x =>
						new RecentAttendanceResponse
						{
							Id =
								x.Id,

							MemberId =
								x.MemberId,

							MemberName =
								BuildFullName(
									x.FirstName,
									x.MiddleName,
									x.LastName),

							EventId =
								x.EventId,

							EventName =
								x.EventName,

							TimeIn =
								x.TimeIn,

							TimeOut =
								x.TimeOut
						})
				.ToList();


		var upcomingRaw =
			await _context.Events
				.AsNoTracking()
				.Where(
					x =>
						x.EventDate >
						today)
				.OrderBy(
					x =>
						x.EventDate)
				.ThenBy(
					x =>
						x.StartTime)
				.Take(
					upcomingEventCount)
				.Select(
					x =>
						new
						{
							x.Id,
							x.Name,
							x.EventDate,
							x.StartTime,
							x.EndTime,
							x.AttendanceTimeInStart,
							x.AttendanceTimeInEnd,
							x.AttendanceTimeOutStart,
							x.AttendanceTimeOutEnd,
							x.Type,
							x.Status
						})
				.ToListAsync(
					cancellationToken);


		var upcomingEvents =
			upcomingRaw
				.Select(
					x =>
						new DashboardEventResponse
						{
							Id =
								x.Id,

							Name =
								x.Name,

							EventDate =
								x.EventDate,

							StartTime =
								x.StartTime,

							EndTime =
								x.EndTime,

							Type =
								x.Type.ToString(),

							Status =
								x.Status.ToString(),
							AttendanceTimeInStart = x.AttendanceTimeInStart,
							AttendanceTimeInEnd = x.AttendanceTimeInEnd,
							AttendanceTimeOutStart = x.AttendanceTimeOutStart,
							AttendanceTimeOutEnd = x.AttendanceTimeOutEnd
						})
				.ToList();


		var attendanceRate =
			activeMemberCount > 0
				? Math.Round(
					(decimal)todayAttendanceCount /
					activeMemberCount *
					100m,
					1)
				: 0m;


		return new AdminDashboardResponse
		{
			Date =
				today,

			ActiveMemberCount =
				activeMemberCount,

			TodayAttendanceCount =
				todayAttendanceCount,

			TodayAttendanceRate =
				attendanceRate,

			PendingRegistrationCount =
				pendingRegistrationCount,

			TodayEventSummary =
				new TodayEventSummaryResponse
				{
					Total =
						todayEvents.Count,

					Scheduled =
						todayEvents.Count(
							x =>
								x.Status ==
								"Scheduled"),

					Ongoing =
						todayEvents.Count(
							x =>
								x.Status ==
								"Ongoing"),

					Completed =
						todayEvents.Count(
							x =>
								x.Status ==
								"Completed")
				},

			TodayEvents =
				todayEvents,

			AttendanceTrend =
				attendanceTrend,

			RecentAttendances =
				recentAttendances,

			UpcomingEvents =
				upcomingEvents
		};
	}


	private static string BuildFullName(
		string firstName,
		string? middleName,
		string lastName)
	{
		var parts =
			new[]
			{
				firstName,
				middleName,
				lastName
			}
			.Where(
				x =>
					!string.IsNullOrWhiteSpace(
						x));


		return string.Join(
			" ",
			parts);
	}
}