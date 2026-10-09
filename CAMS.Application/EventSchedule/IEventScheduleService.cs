using CAMS.Application.Common.Pagination;
using CAMS.Application.EventSchedule.DTOs;

namespace CAMS.Application.EventSchedule;

public interface IEventScheduleService
{
	Task<EventScheduleResponse> GetByIdAsync(
		Guid id,
		CancellationToken cancellationToken = default);

	Task<PagedResult<EventScheduleResponse>> SearchAsync(
		PagedRequest<EventScheduleFilter> request,
		CancellationToken cancellationToken = default);

	Task<EventScheduleResponse> CreateAsync(
		CreateEventScheduleRequest request,
		CancellationToken cancellationToken = default);

	Task<EventScheduleResponse> UpdateAsync(
		Guid id,
		UpdateEventScheduleRequest request,
		CancellationToken cancellationToken = default);

	Task DeleteAsync(
		Guid id,
		CancellationToken cancellationToken = default);
}
