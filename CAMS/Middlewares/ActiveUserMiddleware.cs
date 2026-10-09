using CAMS.Application.Common.Exceptions;
using CAMS.Application.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CAMS.Web.Middlewares;

public class ActiveUserMiddleware
{
	private readonly RequestDelegate _next;

	public ActiveUserMiddleware(
		RequestDelegate next)
	{
		_next =
			next;
	}


	public async Task InvokeAsync(
		HttpContext context,
		IUserService userService)
	{
		if (
			context.User.Identity?.IsAuthenticated !=
			true
		)
		{
			await _next(
				context);

			return;
		}


		var userIdValue =
			context.User.FindFirstValue(
				ClaimTypes.NameIdentifier);


		if (
			!Guid.TryParse(
				userIdValue,
				out var userId)
		)
		{
			await RejectUserAsync(
				context,
				"Unable to determine the authenticated user.",
				"INVALID_USER");

			return;
		}


		var isActive =
			await userService.IsUserActiveAsync(
				userId,
				context.RequestAborted);


		if (!isActive)
		{
			await RejectUserAsync(
				context,
				"This user account has been disabled.",
				"USER_DISABLED");

			return;
		}


		await _next(
			context);
	}


	private static async Task RejectUserAsync(
		HttpContext context,
		string message,
		string code)
	{
		await context.SignOutAsync(
			IdentityConstants.ApplicationScheme);


		if (
			context.Request.Path.StartsWithSegments(
				"/api")
		)
		{
			throw new UnauthorizedException(
				message,
				code);
		}


		context.Response.Redirect(
			"/auth/login");
	}
}