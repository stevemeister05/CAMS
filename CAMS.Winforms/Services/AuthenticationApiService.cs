using CAMS.Application.Authentication;
using CAMS.Winforms.Api;

namespace CAMS.Winforms.Services;

public sealed class AuthenticationApiService
{
	private readonly CamsApiClient
		_apiClient;


	public AuthenticationApiService(
		CamsApiClient apiClient)
	{
		_apiClient =
			apiClient;
	}


	public async Task<LoginResult>
		LoginAsync(
			LoginRequest request,
			CancellationToken cancellationToken = default)
	{
		using var response =
			await _apiClient.PostAsync(
				"/api/v1/auth/login",
				request,
				cancellationToken);


		var payload =
			await _apiClient.ReadAsync<LoginResultPayload>(
				response,
				cancellationToken);


		if (payload is null)
		{
			var message =
				await _apiClient.GetErrorMessageAsync(
					response,
					"Unable to sign in.",
					cancellationToken);


			throw new InvalidOperationException(
				message);
		}


		return MapLoginResult(
			payload);
	}


	private static LoginResult MapLoginResult(
		LoginResultPayload payload)
	{
		return payload.Status switch
		{
			LoginResultStatus.Success =>
				LoginResult.Success(
					payload.MustChangePassword,
					payload.Roles),

			LoginResultStatus.InvalidCredentials =>
				LoginResult.InvalidCredentials(
					payload.Message),

			LoginResultStatus.PendingApproval =>
				LoginResult.PendingApproval(
					payload.Message),

			LoginResultStatus.RegistrationRejected =>
				LoginResult.RegistrationRejected(
					payload.Message),

			LoginResultStatus.Deactivated =>
				LoginResult.Deactivated(
					payload.Message),

			LoginResultStatus.LockedOut =>
				LoginResult.LockedOut(
					payload.Message),

			LoginResultStatus.NotAllowed =>
				LoginResult.NotAllowed(
					payload.Message),

			LoginResultStatus.RequiresTwoFactor =>
				LoginResult.RequiresTwoFactor(
					payload.Message),

			_ =>
				throw new InvalidOperationException(
					"Unknown login result received from the CAMS server.")
		};
	}


	private sealed class LoginResultPayload
	{
		public LoginResultStatus Status { get; set; }

		public string? Message { get; set; }

		public bool MustChangePassword { get; set; }

		public List<string> Roles { get; set; } =
			[];
	}
}