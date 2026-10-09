using CAMS.Application.Authorization;
using CAMS.Application.Event;
using CAMS.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CAMS.Web.Controllers.Api;

[Authorize(Policy = AuthorizationPolicies.PasswordChangeCompleted)]
[Authorize(Roles = ApplicationRoles.Administrator)]
[Route("api/v1/[controller]")]
[ApiController]
public class EventGenerationController : ControllerBase
{
	private readonly IEventGenerationService _eventGenerationService;

	public EventGenerationController(
		IEventGenerationService eventGenerationService)
	{
		_eventGenerationService = eventGenerationService;
	}

	[HttpPost("regenerate")]
	public async Task<IActionResult> Regenerate(
		CancellationToken cancellationToken)
	{
		await _eventGenerationService.RegenerateFutureEventsAsync(
			cancellationToken);

		return NoContent();
	}
}
