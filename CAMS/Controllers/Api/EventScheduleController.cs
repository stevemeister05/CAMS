using CAMS.Application.Authorization;
using CAMS.Application.Common.Pagination;
using CAMS.Application.EventSchedule;
using CAMS.Application.EventSchedule.DTOs;
using CAMS.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CAMS.Web.Controllers.Api;

[ApiController]
[Route("api/v1/event-schedules")]
[Authorize(Roles = ApplicationRoles.Administrator)]
[Authorize(Policy = AuthorizationPolicies.PasswordChangeCompleted)]
public class EventScheduleController : ControllerBase
{
	private readonly IEventScheduleService _eventScheduleService;

	public EventScheduleController(
		IEventScheduleService eventScheduleService)
	{
		_eventScheduleService = eventScheduleService;
	}


	[HttpGet]
	public async Task<ActionResult<
		PagedResult<EventScheduleResponse>>> Search(
			[FromQuery]
			PagedRequest<EventScheduleFilter> request,
			CancellationToken cancellationToken)
	{
		var result =
			await _eventScheduleService.SearchAsync(
				request,
				cancellationToken);


		return Ok(
			result);
	}


	[HttpGet("{id:guid}")]
	public async Task<ActionResult<EventScheduleResponse>>
		GetById(
			Guid id,
			CancellationToken cancellationToken)
	{
		var result =
			await _eventScheduleService.GetByIdAsync(
				id,
				cancellationToken);


		return Ok(
			result);
	}


	[HttpPost]
	public async Task<ActionResult<EventScheduleResponse>>
		Create(
			[FromBody]
			CreateEventScheduleRequest request,
			CancellationToken cancellationToken)
	{
		var result =
			await _eventScheduleService.CreateAsync(
				request,
				cancellationToken);


		return CreatedAtAction(
			nameof(GetById),
			new
			{
				id =
					result.Id
			},
			result);
	}


	[HttpPut("{id:guid}")]
	public async Task<ActionResult<EventScheduleResponse>>
		Update(
			Guid id,
			[FromBody]
			UpdateEventScheduleRequest request,
			CancellationToken cancellationToken)
	{
		var result =
			await _eventScheduleService.UpdateAsync(
				id,
				request,
				cancellationToken);


		return Ok(
			result);
	}


	[HttpDelete("{id:guid}")]
	public async Task<IActionResult> Delete(
		Guid id,
		CancellationToken cancellationToken)
	{
		await _eventScheduleService.DeleteAsync(
			id,
			cancellationToken);


		return NoContent();
	}
}