using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CAMS.Web.Areas.Admin.Controllers;

[Authorize]
[Area("Admin")]
[Route("admin/events")]
[Route("admin/event")]
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
