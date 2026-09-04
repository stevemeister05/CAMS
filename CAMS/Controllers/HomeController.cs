using CAMS.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CAMS.Web.Controllers;

public class HomeController : Controller
{
	[HttpGet("/")]
	[AllowAnonymous]
	public IActionResult Index()
	{
		if (
			User.Identity?.IsAuthenticated !=
			true)
		{
			return View();
		}


		if (
			User.IsInRole(
				ApplicationRoles.Administrator) ||
			User.IsInRole(
				ApplicationRoles.AttendanceStaff)
		)
		{
			return Redirect(
				"/admin");
		}


		if (
			User.IsInRole(
				ApplicationRoles.Member)
		)
		{
			return Redirect(
				"/portal");
		}


		return Redirect(
			"/auth/access-denied");
	//if (
	//	User.Identity?.IsAuthenticated !=
	//	true)
	//{
	//	return Redirect(
	//		"/auth/login");
	//}

	//if (
	//	User.IsInRole(
	//		ApplicationRoles.Administrator) ||
	//	User.IsInRole(
	//		ApplicationRoles.AttendanceStaff)
	//)
	//{
	//	return Redirect(
	//		"/admin");
	//}

	//if (
	//	User.IsInRole(
	//		ApplicationRoles.Member)
	//)
	//{
	//	return Redirect(
	//		"/portal");
	//}

	//return Redirect(
	//	"/auth/access-denied");
	}
}