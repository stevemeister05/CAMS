using CAMS.Application.Attendance;
using CAMS.Application.Attendance.DTOs;
using CAMS.Application.Authorization;
using CAMS.Application.Common.Exceptions;
using CAMS.Application.Member;
using CAMS.Application.Member.DTOs;
using CAMS.Application.User;
using CAMS.Domain.Constants;
using CAMS.Domain.Enums;
using CAMS.Web.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CAMS.Web.Controllers.Api;

[ApiController]
[Route("api/v1/portal")]
[Authorize(
	Roles =
		ApplicationRoles.Member,
	Policy =
		AuthorizationPolicies.PasswordChangeCompleted)]
public class PortalController : ControllerBase
{
	private readonly IUserService _userService;
	private readonly IMemberService _memberService;
	private readonly IAttendanceService _attendanceService;

	public PortalController(
		IUserService userService,
		IMemberService memberService,
		IAttendanceService attendanceService)
	{
		_userService =
			userService;

		_memberService =
			memberService;

		_attendanceService =
			attendanceService;
	}


	[HttpGet("profile")]
	public async Task<IActionResult> GetProfile(
		CancellationToken cancellationToken)
	{
		var userId =
			GetCurrentUserId();


		var user =
			await _userService.GetByIdAsync(
				userId,
				cancellationToken);


		if (!user.MemberId.HasValue)
		{
			throw new ConflictException(
				"This user account is not linked " +
				"to a member.");
		}


		var member =
			await _memberService.GetByIdAsync(
				user.MemberId.Value,
				cancellationToken);


		return Ok(
			ApiResponse<MemberResponse>.Ok(
				member,
				"Member profile retrieved successfully."));
	}

	[HttpGet("attendance-history")]
	public async Task<IActionResult> GetAttendanceHistory(
		CancellationToken cancellationToken)
	{
		var userId =
			GetCurrentUserId();


		var user =
			await _userService.GetByIdAsync(
				userId,
				cancellationToken);


		if (!user.MemberId.HasValue)
		{
			throw new ConflictException(
				"This user account is not linked " +
					"to a member.");
		}


		var result =
			await _attendanceService.GetByMemberAsync(
				user.MemberId.Value,
				cancellationToken);


		return Ok(
			ApiResponse<
				IReadOnlyList<MemberAttendanceHistoryResponse>>
				.Ok(
					result,
					"Attendance history retrieved successfully."));
	}

	[HttpPost("attendance/qr")]
	public async Task<IActionResult> RecordQrAttendance(
		[FromBody] RecordQrAttendanceRequest request,
		CancellationToken cancellationToken)
	{
		var memberId =
			await GetCurrentMemberIdAsync(
				cancellationToken);


		var result =
			await _attendanceService.RecordQrAttendanceAsync(
				memberId,
				request,
				cancellationToken);


		var message =
			result.Action ==
				AttendanceAction.TimeOut
				? "Time-out recorded successfully."
				: "Time-in recorded successfully.";


		return Ok(
			ApiResponse<AttendanceResponse>.Ok(
				result,
				message));
	}


	private Guid GetCurrentUserId()
	{
		var value =
			User.FindFirstValue(
				ClaimTypes.NameIdentifier);


		if (
			!Guid.TryParse(
				value,
				out var userId)
		)
		{
			throw new UnauthorizedException(
				"Unable to determine the authenticated user.");
		}


		return userId;
	}

	private async Task<Guid> GetCurrentMemberIdAsync(
		CancellationToken cancellationToken)
	{
		var userId =
			GetCurrentUserId();


		var user =
			await _userService.GetByIdAsync(
				userId,
				cancellationToken);


		if (!user.MemberId.HasValue)
		{
			throw new ConflictException(
				"This user account is not linked " +
				"to a member.");
		}


		return user.MemberId.Value;
	}
}