namespace CAMS.Application.Authentication;

public sealed class LoginResponse
{
	public LoginResultStatus Status { get; init; }

	public bool Succeeded =>
		Status ==
		LoginResultStatus.Success;

	public string? Message { get; init; }

	public bool MustChangePassword { get; init; }

	public List<string> Roles { get; init; } =
		[];


	public static LoginResponse FromResult(
		LoginResult result)
	{
		return new LoginResponse
		{
			Status =
				result.Status,

			Message =
				result.Message,

			MustChangePassword =
				result.MustChangePassword,

			Roles =
				result.Roles
					.ToList()
		};
	}
}