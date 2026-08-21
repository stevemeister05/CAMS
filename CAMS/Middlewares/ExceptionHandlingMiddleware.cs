using CAMS.Application.Common.Exceptions;
using CAMS.Web.Models.Responses;
using System.Net;
using System.Text.Json;

namespace CAMS.Web.Middlewares;

public sealed class ExceptionHandlingMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger<ExceptionHandlingMiddleware> _logger;

	public ExceptionHandlingMiddleware(
		RequestDelegate next,
		ILogger<ExceptionHandlingMiddleware> logger)
	{
		_next = next;
		_logger = logger;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await _next(context);
		}
		catch (CamsApplicationException exception)
		{
			await HandleApplicationExceptionAsync(
				context,
				exception);
		}
		catch (Exception exception)
		{
			await HandleUnhandledExceptionAsync(
				context,
				exception);
		}
	}

	private static async Task HandleApplicationExceptionAsync(
		HttpContext context,
		CamsApplicationException exception)
	{
		var response = ApiResponse<object>.Fail(
			exception.Message,
			new[]
			{
				new ApiError
				{
					Code = exception.Code,
					Message = exception.Message
				}
			});

		context.Response.StatusCode = exception.StatusCode;
		context.Response.ContentType = "application/json";

		await context.Response.WriteAsJsonAsync(response);
	}

	private async Task HandleUnhandledExceptionAsync(
		HttpContext context,
		Exception exception)
	{
		_logger.LogError(
			exception,
			"An unhandled exception occurred.");
		var response = ApiResponse<object>.Fail(
			exception.Message,
			new[]
			{
				new ApiError
				{
					Code ="INTERNAL_ERROR",
					Message = "An unexpected error occurred."
				}
			});

		context.Response.StatusCode = 500;

		context.Response.ContentType =
			"application/json";

		await context.Response.WriteAsJsonAsync(response);
	}
}
