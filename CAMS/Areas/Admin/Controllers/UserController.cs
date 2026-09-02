using CAMS.Application.Authorization;
using CAMS.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CAMS.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Route("admin/users")]
[Authorize(Roles = ApplicationRoles.Administrator, Policy = AuthorizationPolicies.PasswordChangeCompleted)]
public class UserController : Controller
{
	[HttpGet("")]
	public IActionResult Index()
	{
		return View();
	}
}