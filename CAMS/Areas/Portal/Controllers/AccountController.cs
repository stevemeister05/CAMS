using CAMS.Application.Authorization;
using CAMS.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CAMS.Web.Areas.Portal.Controllers;

[Area("Portal")]
[Route("portal")]
public class AccountController : Controller
{
	[AllowAnonymous]
	[HttpGet("register")]
	public IActionResult Register()
	{
		return View();
	}


	[Authorize(
		Roles =
			ApplicationRoles.Member,
		Policy =
			AuthorizationPolicies.PasswordChangeCompleted)]
	[HttpGet("profile")]
	public IActionResult Profile()
	{
		return View();
	}
}