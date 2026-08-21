using CAMS.Application.Common.Pagination;
using CAMS.Application.Common.RateLimiting;
using CAMS.Application.Registration;
using CAMS.Application.Registration.DTOs;
using CAMS.Web.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CAMS.Web.Controllers.Api;

[Authorize]
[Route("api/[controller]")]
[Route("api/[controller]s")]
[ApiController]
public class RegistrationController : ControllerBase
{
	private readonly IRegistrationService _registrationService;

	public RegistrationController(
		IRegistrationService registrationService)
	{
		_registrationService = registrationService;
	}

	/// <summary>
	/// Submit a self-registration request.
	/// </summary>
	[AllowAnonymous]
	[HttpPost]
	[EnableRateLimiting(RateLimitLevel.High)]
	[ProducesResponseType(typeof(ApiResponse<RegistrationResponse>), StatusCodes.Status201Created)]
	public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
	{
		var registration = await _registrationService.RegisterAsync(request, cancellationToken);

		return StatusCode(StatusCodes.Status201Created, ApiResponse<RegistrationResponse>.Ok(registration));
	}

	/// <summary>
	/// Get a registration request by ID.
	/// </summary>
	[HttpGet("{id:guid}")]
	[EnableRateLimiting(RateLimitLevel.Low)]
	[ProducesResponseType(typeof(ApiResponse<RegistrationResponse>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
	{
		var registration = await _registrationService.GetByIdAsync(id, cancellationToken);

		return Ok(ApiResponse<RegistrationResponse>.Ok(registration));
	}

	/// <summary>
	/// Search registration requests.
	/// </summary>
	[HttpGet]
	[EnableRateLimiting(RateLimitLevel.Moderate)]
	[ProducesResponseType(typeof(ApiResponse<PagedResult<RegistrationResponse>>), StatusCodes.Status200OK)]
	public async Task<IActionResult> Search([FromQuery] PagedRequest<RegistrationFilter> request, CancellationToken cancellationToken)
	{
		var result = await _registrationService.SearchAsync(request, cancellationToken);

		return Ok(ApiResponse<PagedResult<RegistrationResponse>>.Ok(result));
	}

	/// <summary>
	/// Approve a pending registration request.
	/// </summary>
	[HttpPost("{id:guid}/approve")]
	[EnableRateLimiting(RateLimitLevel.High)]
	[ProducesResponseType(typeof(ApiResponse<RegistrationResponse>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
	public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
	{
		var registration = await _registrationService.ApproveAsync(id, cancellationToken);

		return Ok(ApiResponse<RegistrationResponse>.Ok(registration));
	}

	/// <summary>
	/// Reject a pending registration request.
	/// </summary>
	[HttpPost("{id:guid}/reject")]
	[EnableRateLimiting(RateLimitLevel.High)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
	public async Task<IActionResult> Reject(Guid id, [FromBody] RejectRegistrationRequest request, CancellationToken cancellationToken)
	{
		await _registrationService.RejectAsync(id, request, cancellationToken);

		return Ok(
			ApiResponse<object>.Ok(null));
	}
}
