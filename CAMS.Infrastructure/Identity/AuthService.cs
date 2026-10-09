using CAMS.Application.Common.Exceptions;
using CAMS.Application.Registration;
using CAMS.Domain.Enums;
using CAMS.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace CAMS.Application.Authentication;

public sealed class AuthService : IAuthService
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly SignInManager<ApplicationUser> _signInManager;
	private readonly IRegistrationRequestRepository _registrationRequestRepository;

	public AuthService(
		UserManager<ApplicationUser> userManager,
		SignInManager<ApplicationUser> signInManager,
		IRegistrationRequestRepository registrationRequestRepository)
	{
		_userManager = userManager;
		_signInManager = signInManager;
		_registrationRequestRepository = registrationRequestRepository;
	}

	public async Task<LoginResult> LoginAsync(
		LoginRequest request,
		CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(request.UserName) ||
			string.IsNullOrWhiteSpace(request.Password))
		{
			return LoginResult.InvalidCredentials();
		}

		var user = await _userManager.FindByNameAsync(
			request.UserName);

		if (user is null)
		{
			var registrationRequest = await _registrationRequestRepository
				.GetByMobileNumberAsync(
					request.UserName,
					cancellationToken);

			if (registrationRequest != null)
			{
				if (registrationRequest.Status == RegistrationStatus.Pending)
				{
					return LoginResult.PendingApproval();
				}


				if (registrationRequest.Status == RegistrationStatus.Rejected)
				{
					return LoginResult.RegistrationRejected();
				}
			}

			return LoginResult.InvalidCredentials();
		}

		var result = await _signInManager.PasswordSignInAsync(
			user,
			request.Password,
			request.RememberMe,
			lockoutOnFailure: true);

		if (!user.IsActive)
		{
			return LoginResult.Deactivated();
		}

		if (result.Succeeded)
		{
			var roles = await _userManager.GetRolesAsync(
			user);

			return LoginResult.Success(
				mustChangePassword:
					user.MustChangePassword,
				roles: roles.AsReadOnly());
		}

		if (result.IsLockedOut)
			return LoginResult.LockedOut();

		if (result.IsNotAllowed)
			return LoginResult.NotAllowed();

		if (result.RequiresTwoFactor)
			return LoginResult.RequiresTwoFactor();

		return LoginResult.InvalidCredentials();
	}

	public async Task LogoutAsync()
	{
		await _signInManager.SignOutAsync();
	}
}
