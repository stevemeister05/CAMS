using CAMS.Application.Common.RateLimiting;
using CAMS.Web.Models.Responses;
using System.Threading.RateLimiting;

namespace CAMS.Web.Extensions;

public static class RateLimitingExtensions
{
	public static IServiceCollection AddCamsRateLimiting(
		this IServiceCollection services)
	{
		services.AddRateLimiter(options =>
		{
			options.RejectionStatusCode =
				StatusCodes.Status429TooManyRequests;

			options.AddPolicy(
				RateLimitLevel.High,
				CreatePolicy(5));

			options.AddPolicy(
				RateLimitLevel.Moderate,
				CreatePolicy(20));

			options.AddPolicy(
				RateLimitLevel.Low,
				CreatePolicy(60));

			options.OnRejected = async (context, cancellationToken) =>
			{
				context.HttpContext.Response.StatusCode =
					StatusCodes.Status429TooManyRequests;

				await context.HttpContext.Response.WriteAsJsonAsync(
					ApiResponse<object>.Fail(
						$"Too many requests. Please try again in a minute."),
					cancellationToken);
			};
		});

		return services;
	}

	private static Func<
		HttpContext,
		RateLimitPartition<string>>
		CreatePolicy(int permitLimit)
	{
		return httpContext =>
		{
			var ipAddress =
				httpContext.Connection.RemoteIpAddress?.ToString()
				?? "unknown";

			return RateLimitPartition.GetFixedWindowLimiter(
				ipAddress,
				_ => new FixedWindowRateLimiterOptions
				{
					PermitLimit = permitLimit,
					Window = TimeSpan.FromMinutes(1),
					QueueLimit = 0,
					AutoReplenishment = true
				});
		};
	}
}
