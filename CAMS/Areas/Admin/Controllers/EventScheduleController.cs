using CAMS.Application.Authorization;
using CAMS.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CAMS.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Route("admin/event-schedules")]
[Authorize(
	Roles =
		ApplicationRoles.Administrator)]
[Authorize(
	Policy =
		AuthorizationPolicies.PasswordChangeCompleted)]
public class EventScheduleController : Controller
{
	[HttpGet("")]
	public IActionResult Index()
	{
		return View();
	}
}