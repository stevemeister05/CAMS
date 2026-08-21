using CAMS.Application.Event;
using CAMS.Domain.Entities;
using CAMS.Domain.Enums;
using CAMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CAMS.Infrastructure.Repositories;

public class EventRepository : Repository<Event>, IEventRepository
{
	public EventRepository(CAMSDBContext context)
		: base(context)
	{
	}

	public async Task<bool> ExistsAsync(
		EventType type,
		DateOnly eventDate,
		TimeOnly startTime,
		CancellationToken cancellationToken = default)
	{
		return await DbSet.AnyAsync(
			x =>
				x.Type == type &&
				x.EventDate == eventDate &&
				x.StartTime == startTime,
			cancellationToken);
	}

	public async Task<IReadOnlyList<Event>> GetByDateRangeAsync(
		DateOnly? startDate = null,
		DateOnly? endDate = null,
		CancellationToken cancellationToken = default)
	{
		if (startDate is null &&
			endDate is null)
		{
			throw new ArgumentException(
				"At least one date must be specified.",
				nameof(startDate));
		}

		if (startDate.HasValue &&
			endDate.HasValue &&
			startDate.Value > endDate.Value)
		{
			throw new ArgumentException(
				"Start date must be earlier than or equal to the end date.",
				nameof(startDate));
		}

		var query = DbSet
			.AsNoTracking()
			.AsQueryable();

		if (startDate.HasValue)
		{
			query = query.Where(x =>
				x.EventDate >= startDate.Value);
		}

		if (endDate.HasValue)
		{
			query = query.Where(x =>
				x.EventDate <= endDate.Value);
		}

		return await query
			.OrderBy(x => x.EventDate)
			.ThenBy(x => x.StartTime)
			.ToListAsync(cancellationToken);
	}
}
