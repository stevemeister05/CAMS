using CAMS.Application.Attendance;
using CAMS.Application.Attendance.DTOs;
using CAMS.Application.Common.RateLimiting;
using CAMS.Domain.Constants;
using CAMS.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CAMS.Web.Controllers.Api;

[Authorize]
[Route("api/v1/[controller]")]
[ApiController]
[EnableRateLimiting(RateLimitLevel.Low)]
public class AttendanceController : ControllerBase
{
	private readonly IAttendanceService _attendanceService;
	private readonly IAttendanceQrService _attendanceQrService;

	public AttendanceController(
		IAttendanceService attendanceService,
		IAttendanceQrService attendanceQrService)
	{
		_attendanceService =
			attendanceService;
		_attendanceQrService =
			attendanceQrService;
	}

	/// <summary>
	/// Gets attendance information for an event.
	/// </summary>
	[HttpGet("event/{eventId:guid}")]
	[Authorize(Roles = ApplicationRoles.AdministratorOrAttendanceStaff)]
	public async Task<IActionResult> GetAttendanceByEvent(
		Guid eventId,
		CancellationToken cancellationToken)
	{
		var result =
			await _attendanceService
				.GetAttendanceByEventAsync(
					eventId,
					cancellationToken);

		return Ok(result);
	}

	/// <summary>
	/// Records attendance for a member using a QR code.
	/// </summary>
	[HttpGet("qr")]
	public async Task<IActionResult> GetQr(
	[FromQuery] Guid eventId,
	[FromQuery] AttendanceAction action,
	CancellationToken cancellationToken)
	{
		var qr =
			await _attendanceQrService.GetQrCodeAsync(
				eventId,
				action,
				cancellationToken);


		return Ok(
			qr);
	}

	/// <summary>
	/// Records attendance for a member using fingerprint authentication.
	/// </summary>
	[HttpPost("fingerprint")]
	[Authorize(Roles = ApplicationRoles.AdministratorOrAttendanceStaff)]
	public async Task<IActionResult> RecordFingerprintAttendance(
		[FromBody] RecordFingerprintAttendanceRequest request,
		CancellationToken cancellationToken)
	{
		var result =
			await _attendanceService
				.RecordFingerprintAttendanceAsync(
					request,
					cancellationToken);

		return Ok(result);
	}

	/// <summary>
	/// Records attendance for a member manually by an administrator or attendance staff.
	/// </summary>
	[HttpPost("manual")]
	[Authorize(Roles = ApplicationRoles.AdministratorOrAttendanceStaff)]
	public async Task<IActionResult> RecordManualAttendance(
		[FromBody] RecordManualAttendanceRequest request,
		CancellationToken cancellationToken)
	{
		var result =
			await _attendanceService
				.RecordManualAttendanceAsync(
					request,
					cancellationToken);

		return Ok(result);
	}

	private Guid GetCurrentMemberId()
	{
		var memberIdClaim =
			User.FindFirst("MemberId");

		if (memberIdClaim is null ||
			!Guid.TryParse(
				memberIdClaim.Value,
				out var memberId))
		{
			throw new UnauthorizedAccessException(
				"Authenticated member could not be identified.");
		}

		return memberId;
	}
}
