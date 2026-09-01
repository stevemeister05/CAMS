using CAMS.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace CAMS.Application.Authentication;

public sealed class AuthService : IAuthService
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly SignInManager<ApplicationUser> _signInManager;

	public AuthService(
		UserManager<ApplicationUser> userManager,
		SignInManager<ApplicationUser> signInManager)
	{
		_userManager = userManager;
		_signInManager = signInManager;
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
			return LoginResult.InvalidCredentials();
		}

		var result = await _signInManager.PasswordSignInAsync(
			user,
			request.Password,
			request.RememberMe,
			lockoutOnFailure: true);

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
