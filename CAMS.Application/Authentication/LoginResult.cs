namespace CAMS.Application.Authentication;

public sealed class LoginResult
{
	public LoginResultStatus Status { get; }

	public bool Succeeded =>
		Status == LoginResultStatus.Success;

	public string? Message { get; }

	public bool MustChangePassword { get; }

	private LoginResult(
		LoginResultStatus status,
		string? message = null,
		bool mustChangePassword = false)
	{
		Status = status;
		Message = message;
		MustChangePassword = mustChangePassword;
	}

	public static LoginResult Success(
		bool mustChangePassword = false)
		=> new(
			LoginResultStatus.Success,
			mustChangePassword: mustChangePassword);

	public static LoginResult InvalidCredentials(
		string? message = null)
		=> new(
			LoginResultStatus.InvalidCredentials,
			message ?? "Invalid username or password.");

	public static LoginResult LockedOut(
		string? message = null)
		=> new(
			LoginResultStatus.LockedOut,
			message ?? "The account is temporarily locked.");

	public static LoginResult NotAllowed(
		string? message = null)
		=> new(
			LoginResultStatus.NotAllowed,
			message ?? "This account is not allowed to sign in.");

	public static LoginResult RequiresTwoFactor(
		string? message = null)
		=> new(
			LoginResultStatus.RequiresTwoFactor,
			message ?? "Two-factor authentication is required.");
}
