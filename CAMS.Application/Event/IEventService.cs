using CAMS.Application.Common.Pagination;
using CAMS.Application.Event.DTOs;

namespace CAMS.Application.Event;

public interface IEventService
{
	Task<EventResponse> GetByIdAsync(
		Guid id,
		CancellationToken cancellationToken = default);

	Task<PagedResult<EventResponse>> SearchAsync(
		PagedRequest<EventFilter> request,
		CancellationToken cancellationToken = default);

	Task<EventResponse> CreateAsync(
		CreateEventRequest request,
		CancellationToken cancellationToken = default);

	Task<EventResponse> UpdateAsync(
		Guid id,
		UpdateEventRequest request,
		CancellationToken cancellationToken = default);

	Task DeleteAsync(
		Guid id,
		CancellationToken cancellationToken = default);
}
