using CAMS.Application.Authorization;
using CAMS.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CAMS.Web.Areas.Admin.Controllers;

[Authorize]
[Area("Admin")]
[Route("admin/events")]
[Route("admin/event")]
[Authorize(Roles = ApplicationRoles.Administrator)]
[Authorize(Policy = AuthorizationPolicies.PasswordChangeCompleted)]
public class EventController : Controller
{
	[HttpGet]
	public IActionResult Index()
	{
		return View();
	}

	[HttpGet("{eventId:guid}/attendance")]
	public IActionResult Attendance(
		Guid eventId)
	{
		return View(eventId);
	}
}
