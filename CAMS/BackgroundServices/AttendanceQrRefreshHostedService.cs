using CAMS.Application.Attendance;
using CAMS.Application.Common.Clocking;
using CAMS.Application.Common.Realtime;
using CAMS.Web.Realtime;

namespace CAMS.Web.BackgroundServices;

public sealed class AttendanceQrRefreshHostedService
	: BackgroundService
{
	private readonly IServiceScopeFactory
		_scopeFactory;

	private readonly IAttendanceQrSubscriptionRegistry
		_subscriptionRegistry;

	private readonly IApplicationClock
		_clock;

	private readonly ILogger<AttendanceQrRefreshHostedService>
		_logger;

	public AttendanceQrRefreshHostedService(
		IServiceScopeFactory scopeFactory,
		IAttendanceQrSubscriptionRegistry subscriptionRegistry,
		IApplicationClock clock,
		ILogger<AttendanceQrRefreshHostedService> logger)
	{
		_scopeFactory =
			scopeFactory;

		_subscriptionRegistry =
			subscriptionRegistry;

		_clock =
			clock;

		_logger =
			logger;
	}

	protected override async Task ExecuteAsync(
		CancellationToken stoppingToken)
	{
		using var timer =
			new PeriodicTimer(
				TimeSpan.FromSeconds(1));

		while (
			await timer.WaitForNextTickAsync(
				stoppingToken))
		{
			await RefreshExpiredQrCodesAsync(
				stoppingToken);
		}
	}

	private async Task RefreshExpiredQrCodesAsync(
		CancellationToken cancellationToken)
	{
		var expiredTargets =
			_subscriptionRegistry
				.GetExpiredTargets(
					_clock.UtcNow);

		if (expiredTargets.Count == 0)
		{
			return;
		}

		using var scope =
			_scopeFactory.CreateScope();

		var qrService =
			scope.ServiceProvider
				.GetRequiredService<
					IAttendanceQrService>();

		var notifier =
			scope.ServiceProvider
				.GetRequiredService<
					IAttendanceNotifier>();

		foreach (var target in expiredTargets)
		{
			try
			{
				/*
				 * GetQrCodeAsync already knows how to:
				 *
				 * 1. Return a valid existing QR, or
				 * 2. Generate a new token if expired.
				 *
				 * Because the registry only gives us
				 * expired targets here, this will normally
				 * result in regeneration.
				 */
				var qr =
					await qrService.GetQrCodeAsync(
						target.EventId,
						target.Action,
						cancellationToken);

				_subscriptionRegistry.UpdateExpiration(
					target.EventId,
					target.Action,
					qr.ExpiresAt);

				_logger.LogInformation(
					$"QR Code for event {target.EventId} has been updated.");
				await notifier.QrUpdatedAsync(
					qr,
					cancellationToken);
			}
			catch (OperationCanceledException)
				when (cancellationToken.IsCancellationRequested)
			{
				throw;
			}
			catch (Exception exception)
			{
				_logger.LogError(
					exception,
					"Failed to refresh attendance QR code " +
					"for Event {EventId}, Action {Action}.",
					target.EventId,
					target.Action);
			}
		}
	}
}
