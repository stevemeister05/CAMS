using CAMS.Application.Authorization;
using CAMS.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CAMS.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Route("admin")]
[Authorize(Roles = ApplicationRoles.AdministratorOrAttendanceStaff)]
[Authorize(Policy = AuthorizationPolicies.PasswordChangeCompleted)]
public class HomeController : Controller
{
	[HttpGet("")]
	public IActionResult Index()
	{
		return View();
	}
}
