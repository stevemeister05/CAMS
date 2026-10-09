namespace CAMS.Winforms.Api;

public sealed class ApiResult<T>
{
	public bool Success { get; init; }

	public T? Data { get; init; }

	public string? ErrorMessage { get; init; }


	public static ApiResult<T> Ok(
		T data)
	{
		return new ApiResult<T>
		{
			Success =
				true,

			Data =
				data
		};
	}


	public static ApiResult<T> Fail(
		string message)
	{
		return new ApiResult<T>
		{
			Success =
				false,

			ErrorMessage =
				message
		};
	}
}