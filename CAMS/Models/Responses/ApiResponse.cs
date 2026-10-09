using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Web.Models.Responses;

public sealed class ApiResponse<T>
{
	public bool Success { get; init; }

	public string? Message { get; init; }

	public T? Data { get; init; }

	public IReadOnlyList<ApiError>? Errors { get; init; }

	public static ApiResponse<T> Ok(
		T? data,
		string? message = null)
	{
		return new ApiResponse<T>
		{
			Success = true,
			Message = message,
			Data = data,
			Errors = null
		};
	}

	public static ApiResponse<T> Fail(
		string message,
		IReadOnlyList<ApiError>? errors = null)
	{
		return new ApiResponse<T>
		{
			Success = false,
			Message = message,
			Data = default,
			Errors = errors
		};
	}
}
