using CAMS.Web.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CAMS.Web.Controllers;

public class ErrorController : Controller
{
	[Route("/error")]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		var exceptionFeature =
			HttpContext.Features.Get<IExceptionHandlerPathFeature>();

		var exception = exceptionFeature?.Error;

		var model = new ErrorViewModel
		{
			RequestId = HttpContext.TraceIdentifier,
			Path = exceptionFeature?.Path,
			Message = exception?.ToString() ?? string.Empty,
		};

		return View(model);
	}
}