using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CAMS.Web.Areas.Admin.Controllers;

[Authorize]
[Area("Admin")]
[Route("admin/members")]
[Route("admin/member")]
public class MemberController : Controller
{
	[HttpGet]
	public IActionResult Index()
	{
		return View();
	}
}
