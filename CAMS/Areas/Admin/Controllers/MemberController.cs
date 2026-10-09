using CAMS.Application.Authorization;
using CAMS.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CAMS.Web.Areas.Admin.Controllers;

[Authorize]
[Area("Admin")]
[Route("admin/members")]
[Route("admin/member")]
[Authorize(Roles = ApplicationRoles.Administrator)]
[Authorize(Policy = AuthorizationPolicies.PasswordChangeCompleted)]
public class MemberController : Controller
{
	[HttpGet]
	public IActionResult Index()
	{
		return View();
	}
}
