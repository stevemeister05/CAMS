using CAMS.Application.Authorization;
using CAMS.Application.Common.Pagination;
using CAMS.Application.Event;
using CAMS.Application.Event.DTOs;
using CAMS.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CAMS.Web.Controllers.Api;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize(Policy = AuthorizationPolicies.PasswordChangeCompleted)]
public class EventController : ControllerBase
{
	private readonly IEventService _eventService;

	public EventController(
		IEventService eventService)
	{
		_eventService = eventService;
	}

	[HttpGet("{id:guid}")]
	[Authorize(Roles = ApplicationRoles.AdministratorOrAttendanceStaff)]
	public async Task<IActionResult> GetById(
		Guid id,
		CancellationToken cancellationToken)
	{
		var result =
			await _eventService.GetByIdAsync(
				id,
				cancellationToken);

		return Ok(result);
	}

	[HttpGet]
	[Authorize(Roles = ApplicationRoles.AdministratorOrAttendanceStaff)]
	public async Task<IActionResult> Search(
		[FromQuery] PagedRequest<EventFilter> request,
		CancellationToken cancellationToken)
	{
		var result =
			await _eventService.SearchAsync(
				request,
				cancellationToken);

		return Ok(result);
	}

	[HttpPost]
	[Authorize(Roles = ApplicationRoles.Administrator)]
	public async Task<IActionResult> Create(
		[FromBody] CreateEventRequest request,
		CancellationToken cancellationToken)
	{
		var result =
			await _eventService.CreateAsync(
				request,
				cancellationToken);

		return CreatedAtAction(
			nameof(GetById),
			new { id = result.Id },
			result);
	}

	[HttpPut("{id:guid}")]
	[Authorize(Roles = ApplicationRoles.Administrator)]
	public async Task<IActionResult> Update(
		Guid id,
		[FromBody] UpdateEventRequest request,
		CancellationToken cancellationToken)
	{
		var result =
			await _eventService.UpdateAsync(
				id,
				request,
				cancellationToken);

		return Ok(result);
	}

	[HttpDelete("{id:guid}")]
	[Authorize(Roles = ApplicationRoles.Administrator)]
	public async Task<IActionResult> Delete(
		Guid id,
		CancellationToken cancellationToken)
	{
		await _eventService.DeleteAsync(
			id,
			cancellationToken);

		return NoContent();
	}
}
