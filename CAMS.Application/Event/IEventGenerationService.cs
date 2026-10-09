namespace CAMS.Application.Event;

public interface IEventGenerationService
{
	Task GenerateUpcomingEventsAsync(
		CancellationToken cancellationToken = default);

	Task RegenerateFutureEventsAsync(
		CancellationToken cancellationToken = default);
}
