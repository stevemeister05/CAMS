using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace CAMS.Infrastructure.Identity;

public class ApplicationUserClaimsPrincipalFactory
	: UserClaimsPrincipalFactory<
		ApplicationUser,
		IdentityRole<Guid>>
{
	public ApplicationUserClaimsPrincipalFactory(
		UserManager<ApplicationUser> userManager,
		RoleManager<IdentityRole<Guid>> roleManager,
		IOptions<IdentityOptions> optionsAccessor)
		: base(
			userManager,
			roleManager,
			optionsAccessor)
	{
	}

	protected override async Task<ClaimsIdentity>
		GenerateClaimsAsync(
			ApplicationUser user)
	{
		var identity =
			await base.GenerateClaimsAsync(user);

		// ---------------------------------------------------------
		// Member ID
		// ---------------------------------------------------------

		if (user.MemberId.HasValue)
		{
			identity.AddClaim(
				new Claim(
					"MemberId",
					user.MemberId.Value.ToString()));
		}

		// ---------------------------------------------------------
		// Password change requirement
		// ---------------------------------------------------------

		if (user.MustChangePassword)
		{
			identity.AddClaim(
				new Claim(
					"MustChangePassword",
					"true"));
		}

		return identity;
	}
}
