using CAMS.Application.Common.Repositories;
using CAMS.Domain.Enums;

namespace CAMS.Application.Event;

public interface IEventRepository : IRepository<Domain.Entities.Event>
{
	Task<bool> ExistsAsync(
		EventType type,
		DateOnly eventDate,
		TimeOnly startTime,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<Domain.Entities.Event>> GetByDateRangeAsync(
		DateOnly? startDate = null,
		DateOnly? endDate = null,
		CancellationToken cancellationToken = default);
}
