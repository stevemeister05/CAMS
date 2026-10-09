using CAMS.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CAMS.Web.Controllers;

[Route("[controller]")]
public class AuthController : Controller
{
    // GET: Auth
    public IActionResult auth404()
    {
        return View();
    }

    public IActionResult auth500()
    {
        return View();
    }

	[HttpGet("auth503")]
	public IActionResult auth503()
    {
        return View();
    }

    public IActionResult LockScreen()
    {
        return View();
    }

    public IActionResult Logout()
    {
        return View();
    }

    public IActionResult Offline()
    {
        return View();
    }

	[HttpGet("access-denied")]
	[AllowAnonymous]
	public IActionResult AccessDenied()
	{
		Response.StatusCode =
			StatusCodes.Status403Forbidden;

		return View();
	}

	[Authorize]
	[HttpGet("change-password")]
	public IActionResult ChangePassword(string? reason = null)
	{
		var displayName =
			User.FindFirst(
				"DisplayName")?.Value ??
			User.Identity?.Name ??
			"there";


		string? message =
			TempData[
				"ChangePasswordMessage"] as string;


		if (
			string.IsNullOrWhiteSpace(
				message) &&
			string.Equals(
				reason,
				"required",
				StringComparison.OrdinalIgnoreCase)
		)
		{
			message =
				"Your account requires you to change your password before you can continue.";
		}


		ViewData["DisplayName"] =
			displayName;

		ViewData["ChangePasswordMessage"] =
			message;


		return View();
	}

	public IActionResult reset()
    {
        return View();
    }

	[AllowAnonymous]
	[HttpGet("login")]
	public IActionResult Login(string? returnUrl = null)
	{
		if (User.Identity?.IsAuthenticated == true)
		{
			if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
			{
				return LocalRedirect(returnUrl);
			}

			if (User.IsInRole(ApplicationRoles.Administrator) || User.IsInRole(ApplicationRoles.AttendanceStaff))
			{
				return Redirect("/admin");
			}

			if (User.IsInRole(ApplicationRoles.Member))
			{
				return Redirect("/portal");
			}

			return Redirect("/");
		}

		ViewData["ReturnUrl"] = returnUrl;

		return View();
	}

	public IActionResult signup()
    {
        return View();
    }

    public IActionResult successmsg()
    {
        return View();
    }

    public IActionResult twostep()
    {
        return View();
    }
}