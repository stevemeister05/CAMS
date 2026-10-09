using CAMS.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Area("Admin")]
[Route("admin/registrations")]
[Authorize(
	Roles =
		ApplicationRoles.Administrator)]
public class RegistrationController : Controller
{
	[HttpGet("")]
	public IActionResult Index()
	{
		return View();
	}

	[HttpGet("{id:guid}")]
	public IActionResult Details(
		Guid id)
	{
		return View(
			id);
	}
}