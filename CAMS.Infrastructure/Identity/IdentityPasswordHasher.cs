using CAMS.Application.Common.Security;
using Microsoft.AspNetCore.Identity;

namespace CAMS.Infrastructure.Identity;

public class IdentityPasswordHasher
	: IPasswordHasher
{
	private readonly IPasswordHasher<ApplicationUser>
		_passwordHasher;

	public IdentityPasswordHasher(
		IPasswordHasher<ApplicationUser> passwordHasher)
	{
		_passwordHasher = passwordHasher;
	}

	public string Hash(string password)
	{
		if (string.IsNullOrWhiteSpace(password))
		{
			throw new ArgumentException(
				"Password is required.",
				nameof(password));
		}

		var user = new ApplicationUser();

		return _passwordHasher.HashPassword(
			user,
			password);
	}

	public bool Verify(
		string password,
		string passwordHash)
	{
		if (string.IsNullOrWhiteSpace(password))
		{
			return false;
		}

		if (string.IsNullOrWhiteSpace(passwordHash))
		{
			return false;
		}

		var user = new ApplicationUser();

		var result =
			_passwordHasher.VerifyHashedPassword(
				user,
				passwordHash,
				password);

		return result != PasswordVerificationResult.Failed;
	}
}
