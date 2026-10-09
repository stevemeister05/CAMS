using CAMS.Application.Report;
using CAMS.Application.Report.DTOs;
using CAMS.Domain.Enums;
using CAMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CAMS.Infrastructure.Repositories;

public sealed class ReportRepository
	: IReportRepository
{
	private readonly CAMSDBContext
		_dbContext;


	public ReportRepository(
		CAMSDBContext dbContext)
	{
		_dbContext =
			dbContext;
	}


	public async Task<
		IReadOnlyList<Domain.Entities.Attendance>>
		GetMemberAttendanceAsync(
			DateOnly dateFrom,
			DateOnly dateTo,
			CancellationToken cancellationToken = default)
	{
		return await _dbContext.Attendances
			.AsNoTracking()
			.Include(x =>
				x.Member)
			.Include(x =>
				x.Event)
			.Where(x =>
				x.Event.EventDate >= dateFrom &&
				x.Event.EventDate <= dateTo)
			.OrderBy(x =>
				x.Event.EventDate)
			.ThenBy(x =>
				x.Event.Name)
			.ThenBy(x =>
				x.Member.LastName)
			.ThenBy(x =>
				x.Member.FirstName)
			.ToListAsync(
				cancellationToken);
	}

	public async Task<IReadOnlyList<MemberAttendanceCountReportResponse>> GetMemberAttendanceCountAsync(
		DateOnly dateFrom,
		DateOnly dateTo,
		int? minimumCount,
		int? maximumCount,
		CancellationToken cancellationToken = default)
	{
		var query =
			_dbContext.Members
				.AsNoTracking()
				.Select(member =>
					new
					{
						Member =
							member,

						AttendanceCount =
							member.Attendances.Count(
								attendance =>
									attendance.Event.EventDate >=
										dateFrom &&
									attendance.Event.EventDate <=
										dateTo)
					});


		if (minimumCount.HasValue)
		{
			query =
				query.Where(x =>
					x.AttendanceCount >=
					minimumCount.Value);
		}


		if (maximumCount.HasValue)
		{
			query =
				query.Where(x =>
					x.AttendanceCount <=
					maximumCount.Value);
		}


		var results =
			await query
				.OrderByDescending(x =>
					x.AttendanceCount)
				.ThenBy(x =>
					x.Member.LastName)
				.ThenBy(x =>
					x.Member.FirstName)
				.ToListAsync(
					cancellationToken);


		return results
			.Select(x =>
				new MemberAttendanceCountReportResponse
				{
					MemberId =
						x.Member.Id,

					MemberName =
						x.Member
							.GetFullNameLastFirst(),

					AttendanceCount =
						x.AttendanceCount
				})
			.ToList();
	}

	public async Task< IReadOnlyList<EventAttendanceSummaryReportResponse>> GetEventAttendanceSummaryAsync(
		DateOnly dateFrom,
		DateOnly dateTo,
		CancellationToken cancellationToken = default)
	{
		return await _dbContext.Events
			.AsNoTracking()
			.Where(@event =>
				@event.EventDate >= dateFrom &&
				@event.EventDate <= dateTo
				&& @event.Status == EventStatus.Completed)
			.OrderBy(@event =>
				@event.EventDate)
			.ThenBy(@event =>
				@event.StartTime)
			.ThenBy(@event =>
				@event.Name)
			.Select(@event =>
				new EventAttendanceSummaryReportResponse
				{
					EventId =
						@event.Id,

					EventName =
						@event.Name,

					EventDate =
						@event.EventDate,

					TotalAttendance =
						_dbContext.Attendances.Count(
							attendance =>
								attendance.EventId ==
								@event.Id)
				})
			.ToListAsync(
				cancellationToken);
	}
}