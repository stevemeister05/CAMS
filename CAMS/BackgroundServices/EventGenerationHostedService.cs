using CAMS.Application.Common.Scheduling;
using CAMS.Application.Event;
using CAMS.Application.Settings;
using Microsoft.Extensions.Options;

namespace CAMS.Web.BackgroundServices;

public sealed class EventGenerationHostedService : BackgroundService
{
	private readonly IServiceScopeFactory _scopeFactory;
	private readonly EventGenerationSettings _settings;
	private readonly ILogger<EventGenerationHostedService> _logger;

	public EventGenerationHostedService(
		IServiceScopeFactory scopeFactory,
		IOptions<EventGenerationSettings> options,
		ILogger<EventGenerationHostedService> logger)
	{
		_scopeFactory = scopeFactory;
		_settings = options.Value;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(
		CancellationToken stoppingToken)
	{
		_logger.LogInformation(
			"Event generation background service started.");

		// Generate immediately when the application starts.
		await GenerateEventsAsync(stoppingToken);

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				var now = DateTime.Now;

				var nextOccurrence =
					ScheduleCalculator.GetNextOccurrence(
						_settings.Schedule,
						now);

				if (nextOccurrence is null)
				{
					_logger.LogWarning(
						"Unable to determine the next event generation schedule.");

					await Task.Delay(
						TimeSpan.FromMinutes(1),
						stoppingToken);

					continue;
				}

				var delay = nextOccurrence.Value - now;

				if (delay < TimeSpan.Zero)
				{
					delay = TimeSpan.Zero;
				}

				_logger.LogInformation("Next event generation scheduled for {NextOccurrence}.", nextOccurrence.Value);

				await Task.Delay(
					delay,
					stoppingToken);

				if (stoppingToken.IsCancellationRequested)
				{
					break;
				}

				await GenerateEventsAsync(stoppingToken);
			}
			catch (OperationCanceledException)
				when (stoppingToken.IsCancellationRequested)
			{
				break;
			}
			catch (Exception ex)
			{
				_logger.LogError(
					ex,
					"An error occurred while running the event generation background service.");

				// Prevent a configuration or runtime error from
				// causing the hosted service to continuously loop.
				try
				{
					await Task.Delay(
						TimeSpan.FromMinutes(1),
						stoppingToken);
				}
				catch (OperationCanceledException)
				{
					break;
				}
			}
		}

		_logger.LogInformation(
			"Event generation background service stopped.");
	}

	private async Task GenerateEventsAsync(
		CancellationToken cancellationToken)
	{
		try
		{
			using var scope = _scopeFactory.CreateScope();

			var eventGenerationService =
				scope.ServiceProvider
					.GetRequiredService<IEventGenerationService>();

			_logger.LogInformation(
				"Starting automatic event generation.");

			await eventGenerationService.GenerateUpcomingEventsAsync(
				cancellationToken);

			_logger.LogInformation(
				"Automatic event generation completed.");
		}
		catch (OperationCanceledException)
			when (cancellationToken.IsCancellationRequested)
		{
			throw;
		}
		catch (Exception ex)
		{
			_logger.LogError(
				ex,
				"Automatic event generation failed.");
		}
	}
}
