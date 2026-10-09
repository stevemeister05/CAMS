using CAMS.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CAMS.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Route("admin/reports")]
[Authorize(
	Roles =
		ApplicationRoles.Administrator)]
public sealed class ReportController
	: Controller
{
	[HttpGet("member-attendance")]
	public IActionResult MemberAttendance()
	{
		return View();
	}

	[HttpGet("member-attendance-count")]
	public IActionResult MemberAttendanceCount()
	{
		return View();
	}

	[HttpGet("event-attendance-summary")]
	public IActionResult EventAttendanceSummary()
	{
		return View();
	}

	[HttpGet("individual-event-attendance")]
	public IActionResult IndividualEventAttendance()
	{
		return View();
	}
}