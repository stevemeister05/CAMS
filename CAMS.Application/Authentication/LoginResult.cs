namespace CAMS.Application.Authentication;

public sealed class LoginResult
{
	public LoginResultStatus Status { get; }

	public bool Succeeded =>
		Status == LoginResultStatus.Success;

	public string? Message { get; }

	public bool MustChangePassword { get; }

	public IReadOnlyList<string> Roles { get; }

	private LoginResult(
		LoginResultStatus status,
		string? message = null,
		bool mustChangePassword = false,
		IReadOnlyList<string>? roles = null)
	{
		Status =
			status;

		Message =
			message;

		MustChangePassword =
			mustChangePassword;

		Roles =
			roles ??
			Array.Empty<string>();
	}

	public static LoginResult Success(
		bool mustChangePassword = false,
		IReadOnlyList<string>? roles = null)
		=> new(
			LoginResultStatus.Success,
			mustChangePassword:
				mustChangePassword,
			roles:
				roles);

	public static LoginResult InvalidCredentials(
		string? message = null)
		=> new(
			LoginResultStatus.InvalidCredentials,
			message ??
				"Invalid username or password.");

	public static LoginResult PendingApproval(
		string? message = null)
		=> new(
			LoginResultStatus.PendingApproval,
			message ??
				"Your registration is still pending approval.");

	public static LoginResult RegistrationRejected(
		string? message = null)
		=> new(
			LoginResultStatus.RegistrationRejected,
			message ??
				"Your registration request was not approved.");

	public static LoginResult Deactivated(
		string? message = null)
		=> new(
			LoginResultStatus.Deactivated,
			message ??
				"This user account has been disabled.");

	public static LoginResult LockedOut(
		string? message = null)
		=> new(
			LoginResultStatus.LockedOut,
			message ??
				"The account is temporarily locked.");

	public static LoginResult NotAllowed(
		string? message = null)
		=> new(
			LoginResultStatus.NotAllowed,
			message ??
				"This account is not allowed to sign in.");

	public static LoginResult RequiresTwoFactor(
		string? message = null)
		=> new(
			LoginResultStatus.RequiresTwoFactor,
			message ??
				"Two-factor authentication is required.");
}