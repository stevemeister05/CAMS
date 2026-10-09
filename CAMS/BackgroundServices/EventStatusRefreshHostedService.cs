using CAMS.Application.Event;

namespace CAMS.Web.BackgroundServices;

public sealed class EventStatusRefreshHostedService :
	BackgroundService
{
	private readonly IServiceScopeFactory _scopeFactory;
	private readonly ILogger<EventStatusRefreshHostedService> _logger;


	public EventStatusRefreshHostedService(
		IServiceScopeFactory scopeFactory,
		ILogger<EventStatusRefreshHostedService> logger)
	{
		_scopeFactory =
			scopeFactory;

		_logger =
			logger;
	}


	protected override async Task ExecuteAsync(
		CancellationToken stoppingToken)
	{
		/*
		 * Reconcile statuses immediately when
		 * the application starts.
		 */
		await UpdateStatusesAsync(
			stoppingToken);


		using var timer =
			new PeriodicTimer(
				TimeSpan.FromSeconds(
					30));


		while (
			await timer.WaitForNextTickAsync(
				stoppingToken)
		)
		{
			await UpdateStatusesAsync(
				stoppingToken);
		}
	}


	private async Task UpdateStatusesAsync(
		CancellationToken cancellationToken)
	{
		try
		{
			using var scope =
				_scopeFactory.CreateScope();


			var service =
				scope.ServiceProvider
					.GetRequiredService<IEventStatusService>();

			_logger.LogInformation("Updating event statuses.");

			await service.UpdateStatusesAsync(
				cancellationToken);
		}
		catch (
			OperationCanceledException)
			when (
				cancellationToken
					.IsCancellationRequested)
		{
			// Application is shutting down.
		}
		catch (Exception exception)
		{
			_logger.LogError(
				exception,
				"An error occurred while updating event statuses.");
		}
	}
}