using CAMS.Application.Authentication;
using CAMS.Application.Common.RateLimiting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CAMS.Web.Controllers.Api;

[Route("api/v1/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
	private readonly IAuthService _authService;

	public AuthController(IAuthService authService)
	{
		_authService = authService;
	}

	[HttpPost("login")]
	[EnableRateLimiting(RateLimitLevel.High)]
	[AllowAnonymous]
	public async Task<IActionResult> Login(
		[FromBody] LoginRequest request,
		CancellationToken cancellationToken)
	{
		var result = await _authService.LoginAsync(
			request,
			cancellationToken);

		if (result.Succeeded)
		{
			return Ok(result);
		}

		return result.Status switch
		{
			LoginResultStatus.LockedOut =>
				StatusCode(
					StatusCodes.Status423Locked,
					result),

			LoginResultStatus.NotAllowed =>
				Forbid(),

			LoginResultStatus.RequiresTwoFactor =>
				Unauthorized(result),

			_ =>
				Unauthorized(result)
		};
	}

	[HttpPost("logout")]
	[EnableRateLimiting(RateLimitLevel.High)]
	[Authorize]
	public async Task<IActionResult> Logout()
	{
		await _authService.LogoutAsync();

		return NoContent();
	}
}
