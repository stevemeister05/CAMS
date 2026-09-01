using CAMS.Application.Authorization;
using CAMS.Application.Common.RateLimiting;
using CAMS.Application.Dashboard;
using CAMS.Application.Dashboard.DTOs;
using CAMS.Domain.Constants;
using CAMS.Web.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CAMS.Web.Controllers.Api;

[Authorize]
[Route("api/v1/dashboard")]
[Authorize(
	Policy =
		AuthorizationPolicies.PasswordChangeCompleted)]
[Authorize(
	Roles =
		ApplicationRoles.Administrator)]
[ApiController]
public class DashboardController : ControllerBase
{
	private readonly IDashboardService _dashboardService;


	public DashboardController(
		IDashboardService dashboardService)
	{
		_dashboardService =
			dashboardService;
	}


	[HttpGet]
	[EnableRateLimiting(
		RateLimitLevel.Low)]
	[ProducesResponseType(
		typeof(
			ApiResponse<AdminDashboardResponse>),
		StatusCodes.Status200OK)]
	public async Task<IActionResult> Get(
		CancellationToken cancellationToken)
	{
		var dashboard =
			await _dashboardService
				.GetAdminDashboardAsync(
					cancellationToken);


		return Ok(
			ApiResponse<AdminDashboardResponse>.Ok(
				dashboard,
				"Dashboard retrieved successfully."));
	}
}