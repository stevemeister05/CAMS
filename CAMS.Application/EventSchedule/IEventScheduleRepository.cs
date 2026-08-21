using CAMS.Application.Common.Repositories;

namespace CAMS.Application.EventSchedule;

public interface IEventScheduleRepository
	: IRepository<Domain.Entities.EventSchedule>
{
	Task<Domain.Entities.EventSchedule?> GetByNameAsync(
		string name,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<Domain.Entities.EventSchedule>> GetActiveSchedulesAsync(
		CancellationToken cancellationToken = default);
}
