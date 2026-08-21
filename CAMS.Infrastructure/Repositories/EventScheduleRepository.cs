using CAMS.Application.EventSchedule;
using CAMS.Domain.Entities;
using CAMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CAMS.Infrastructure.Repositories;

public class EventScheduleRepository
	: Repository<EventSchedule>,
	  IEventScheduleRepository
{
	public EventScheduleRepository(CAMSDBContext context)
		: base(context)
	{
	}

	public async Task<EventSchedule?> GetByNameAsync(
		string name,
		CancellationToken cancellationToken = default)
	{
		return await DbSet
			.FirstOrDefaultAsync(
				x => x.Name == name,
				cancellationToken);
	}

	public async Task<IReadOnlyList<EventSchedule>> GetActiveSchedulesAsync(
		CancellationToken cancellationToken = default)
	{
		return await DbSet
			.AsNoTracking()
			.Where(x => x.IsActive)
			.OrderBy(x => x.StartTime)
			.ToListAsync(cancellationToken);
	}
}
