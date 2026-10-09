using CAMS.Web.Constants;

namespace CAMS.Web.Extensions;

public static class AuthenticationExtensions
{
	public static IServiceCollection AddCamsAuthentication(
		this IServiceCollection services)
	{
		services.ConfigureApplicationCookie(options =>
		{
			options.LoginPath = "/auth/login";
			options.AccessDeniedPath = "/auth/access-denied";

			options.Events.OnRedirectToLogin =
				async context =>
				{
					if (context.Request.Path
						.StartsWithSegments("/api"))
					{
						await WriteApiErrorAsync(
							context.HttpContext,
							StatusCodes.Status401Unauthorized,
							"Authentication required",
							ApiErrorCodes.Unauthorized,
							"You must be authenticated to access this resource.");

						return;
					}

					context.Response.Redirect(
						context.RedirectUri);
				};

			options.Events.OnRedirectToAccessDenied =
				async context =>
				{
					if (context.Request.Path
						.StartsWithSegments("/api"))
					{
						await WriteApiErrorAsync(
							context.HttpContext,
							StatusCodes.Status403Forbidden,
							"Access denied",
							ApiErrorCodes.Forbidden,
							"You do not have permission to access this resource.");

						return;
					}

					context.Response.Redirect(
						context.RedirectUri);
				};
		});

		return services;
	}

	private static async Task WriteApiErrorAsync(
		HttpContext context,
		int statusCode,
		string title,
		string code,
		string detail)
	{
		context.Response.StatusCode =
			statusCode;

		context.Response.ContentType =
			"application/problem+json";

		await context.Response.WriteAsJsonAsync(
			new
			{
				type = $"https://cams.example.com/problems/{code.ToLowerInvariant()}",
				title,
				status = statusCode,
				code,
				detail
			});
	}
}
