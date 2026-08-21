using CAMS.Application.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Infrastructure.Identity;

public sealed class PasswordChangeCompletedHandler
	: AuthorizationHandler<PasswordChangeCompletedRequirement>
{
	private readonly UserManager<ApplicationUser> _userManager;

	public PasswordChangeCompletedHandler(
		UserManager<ApplicationUser> userManager)
	{
		_userManager = userManager;
	}

	protected override async Task HandleRequirementAsync(
		AuthorizationHandlerContext context,
		PasswordChangeCompletedRequirement requirement)
	{
		if (context.User.Identity?.IsAuthenticated != true)
		{
			return;
		}

		var userIdClaim =
			context.User.FindFirst(
				System.Security.Claims.ClaimTypes.NameIdentifier);

		if (userIdClaim is null ||
			!Guid.TryParse(
				userIdClaim.Value,
				out var userId))
		{
			return;
		}

		var user =
			await _userManager.FindByIdAsync(
				userId.ToString());

		if (user is null)
		{
			return;
		}

		if (!user.MustChangePassword)
		{
			context.Succeed(requirement);
		}
	}
}
