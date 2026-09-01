using CAMS.Application.User;
using CAMS.Application.User.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CAMS.Web.Controllers.Api;

[ApiController]
[Route("api/v1/[controller]")]
[Route("api/v1/[controller]s")]
[Authorize]
public class UserController : ControllerBase
{
	private readonly IUserService _userService;

	public UserController(
		IUserService userService)
	{
		_userService = userService;
	}

	/// <summary>
	/// Creates an administrator account.
	/// </summary>
	[HttpPost("administrators")]
	[Authorize(Roles = "Administrator")]
	public async Task<ActionResult<UserResponse>> CreateAdministrator(
		[FromBody] CreateStaffUserRequest request,
		CancellationToken cancellationToken)
	{
		var result =
			await _userService.CreateAdministratorAsync(
				request,
				cancellationToken);

		return Ok(result);
	}

	/// <summary>
	/// Creates an attendance staff account.
	/// </summary>
	[HttpPost("attendance-staff")]
	[Authorize(Roles = "Administrator")]
	public async Task<ActionResult<UserResponse>> CreateAttendanceStaff(
		[FromBody] CreateStaffUserRequest request,
		CancellationToken cancellationToken)
	{
		var result =
			await _userService.CreateAttendanceStaffAsync(
				request,
				cancellationToken);

		return Ok(result);
	}

	/// <summary>
	/// Changes the currently authenticated user's password.
	/// </summary>
	[HttpPost("change-password")]
	public async Task<IActionResult> ChangePassword(
		[FromBody] ChangePasswordRequest request,
		CancellationToken cancellationToken)
	{
		var userIdClaim =
			User.FindFirst(
				System.Security.Claims.ClaimTypes.NameIdentifier);

		if (userIdClaim is null ||
			!Guid.TryParse(
				userIdClaim.Value,
				out var userId))
		{
			return Unauthorized();
		}

		await _userService.ChangePasswordAsync(
			userId,
			request.CurrentPassword,
			request.NewPassword,
			cancellationToken);

		return NoContent();
	}
}
