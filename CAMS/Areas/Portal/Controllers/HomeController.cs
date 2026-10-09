using CAMS.Application.Authorization;
using CAMS.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CAMS.Web.Areas.Portal.Controllers;

[Area("Portal")]
[Route("portal")]
[Authorize(Roles = ApplicationRoles.Member)]
public class HomeController : Controller
{
	[HttpGet("")]
	public IActionResult Index()
	{
		return View();
	}
}