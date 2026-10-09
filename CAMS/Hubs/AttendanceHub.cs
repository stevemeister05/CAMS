using CAMS.Application.Attendance;
using CAMS.Domain.Constants;
using CAMS.Domain.Enums;
using CAMS.Web.Realtime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CAMS.Web.Hubs;

[Authorize(Roles = ApplicationRoles.AdministratorOrAttendanceStaff)]
public class AttendanceHub : Hub
{
	private readonly IAttendanceQrService
		_qrService;

	private readonly IAttendanceQrSubscriptionRegistry
		_subscriptionRegistry;

	public AttendanceHub(
		IAttendanceQrService qrService,
		IAttendanceQrSubscriptionRegistry subscriptionRegistry)
	{
		_qrService =
			qrService;

		_subscriptionRegistry =
			subscriptionRegistry;
	}

	public async Task JoinEvent(
		Guid eventId,
		AttendanceAction action)
	{
		if (eventId == Guid.Empty)
		{
			throw new HubException(
				"Event ID is required.");
		}

		if (!Enum.IsDefined(action))
		{
			throw new HubException(
				"Invalid attendance action.");
		}

		await Groups.AddToGroupAsync(
			Context.ConnectionId,
			GetEventGroupName(eventId),
			Context.ConnectionAborted);

		var qr =
			await _qrService.GetQrCodeAsync(
				eventId,
				action,
				Context.ConnectionAborted);

		/*
		 * Tell the QR background service that this
		 * Event/Action combination is actively being
		 * displayed by this connection.
		 */
		_subscriptionRegistry.Set(
			Context.ConnectionId,
			eventId,
			action,
			qr.ExpiresAt);

		await Clients.Caller.SendAsync(
			"QrUpdated",
			qr,
			Context.ConnectionAborted);
	}

	public async Task LeaveEvent(
		Guid eventId)
	{
		if (eventId == Guid.Empty)
		{
			throw new HubException(
				"Event ID is required.");
		}

		_subscriptionRegistry.Remove(
			Context.ConnectionId);

		await Groups.RemoveFromGroupAsync(
			Context.ConnectionId,
			GetEventGroupName(eventId),
			Context.ConnectionAborted);
	}

	public override async Task OnDisconnectedAsync(
		Exception? exception)
	{
		_subscriptionRegistry.Remove(
			Context.ConnectionId);

		await base.OnDisconnectedAsync(
			exception);
	}

	public static string GetEventGroupName(
		Guid eventId)
	{
		return $"attendance:event:{eventId}";
	}
}