using CAMS.Application.Authorization;
using CAMS.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CAMS.Web.Areas.Portal.Controllers;

[Area("Portal")]
[Route("portal/attendance")]
[Authorize(
	Roles =
		ApplicationRoles.Member,
	Policy =
		AuthorizationPolicies.PasswordChangeCompleted)]
public class AttendanceController : Controller
{
	[HttpGet("history")]
	public IActionResult History()
	{
		return View();
	}

	[HttpGet("scan")]
	public IActionResult Scan()
	{
		return View();
	}
}
