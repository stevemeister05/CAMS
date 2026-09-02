using CAMS.Application.User;
using CAMS.Application.User.DTOs;
using CAMS.Domain.Constants;
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
	[Authorize(Roles = ApplicationRoles.Administrator)]
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
	[Authorize(Roles = ApplicationRoles.Administrator)]
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

	[HttpGet]
	[Authorize(Roles = ApplicationRoles.Administrator)]
	public async Task<ActionResult<
		IReadOnlyList<UserResponse>>> GetAll(CancellationToken cancellationToken)
	{
		var result =
			await _userService.GetAllAsync(
				cancellationToken);


		return Ok(
			result);
	}

	[HttpGet("{id:guid}")]
	[Authorize(Roles = ApplicationRoles.Administrator)]
	public async Task<ActionResult<UserResponse>> GetById(
		Guid id,
		CancellationToken cancellationToken)
	{
		var result =
			await _userService.GetByIdAsync(
				id,
				cancellationToken);


		return Ok(
			result);
	}

	[HttpPut("{id:guid}")]
	[Authorize(Roles = ApplicationRoles.Administrator)]
	public async Task<ActionResult<UserResponse>> Update(
		Guid id,
		[FromBody] UpdateStaffUserRequest request,
		CancellationToken cancellationToken)
	{
		var result =
			await _userService.UpdateStaffUserAsync(
				id,
				request,
				cancellationToken);


		return Ok(
			result);
	}

	[HttpPost("{id:guid}/reset-password")]
	[Authorize(Roles = ApplicationRoles.Administrator)]
	public async Task<IActionResult> ResetPassword(
		Guid id,
		CancellationToken cancellationToken)
	{
		await _userService.ResetPasswordAsync(
			id,
			cancellationToken);


		return NoContent();
	}

	[HttpPut("{id:guid}/status")]
	[Authorize(
	Roles =
		ApplicationRoles.Administrator)]
	public async Task<IActionResult> UpdateStatus(
		Guid id,
		[FromBody]
		UpdateUserStatusRequest request,
		CancellationToken cancellationToken)
	{
		await _userService.SetActiveStatusAsync(
			id,
			request.IsActive,
			cancellationToken);


		return NoContent();
	}
}
