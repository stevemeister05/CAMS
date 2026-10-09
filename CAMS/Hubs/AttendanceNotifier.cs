using CAMS.Application.Attendance.DTOs;
using CAMS.Application.Common.Realtime;
using Microsoft.AspNetCore.SignalR;

namespace CAMS.Web.Hubs;

public sealed class AttendanceNotifier
	: IAttendanceNotifier
{
	private readonly IHubContext<AttendanceHub>
		_hubContext;

	public AttendanceNotifier(
		IHubContext<AttendanceHub> hubContext)
	{
		_hubContext = hubContext;
	}

	public async Task AttendanceRecordedAsync(
		AttendanceResponse attendance,
		CancellationToken cancellationToken = default)
	{
		await _hubContext
			.Clients
			.Group(
				AttendanceHub.GetEventGroupName(
					attendance.EventId))
			.SendAsync(
				"AttendanceRecorded",
				attendance,
				cancellationToken);
	}

	public async Task QrUpdatedAsync(
		AttendanceQrResponse qr,
		CancellationToken cancellationToken = default)
	{
		await _hubContext
			.Clients
			.Group(
				AttendanceHub.GetEventGroupName(
					qr.EventId))
			.SendAsync(
				"QrUpdated",
				qr,
				cancellationToken);
	}
}
