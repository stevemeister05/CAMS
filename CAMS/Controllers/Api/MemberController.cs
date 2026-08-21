using CAMS.Application.Authorization;
using CAMS.Application.Common.Pagination;
using CAMS.Application.Common.RateLimiting;
using CAMS.Application.Member;
using CAMS.Application.Member.DTOs;
using CAMS.Domain.Constants;
using CAMS.Web.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CAMS.Web.Controllers.Api;

[Authorize]
[Route("api/v1/[controller]")]
[Route("api/v1/[controller]s")]
[Authorize(Policy = AuthorizationPolicies.PasswordChangeCompleted)]
[Authorize(Roles = ApplicationRoles.Administrator)]
[ApiController]
public class MemberController : ControllerBase
{
    private readonly IMemberService _memberService;

    public MemberController(IMemberService memberService)
    {
        _memberService = memberService;
    }

    [HttpGet("{id:guid}")]
    [EnableRateLimiting(RateLimitLevel.Low)]
	[ProducesResponseType(
        typeof(ApiResponse<MemberResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var member = await _memberService.GetByIdAsync(
            id,
            cancellationToken);

        return Ok(
            ApiResponse<MemberResponse>.Ok(
                member,
                "Member retrieved successfully."));
    }

    [HttpGet]
    [EnableRateLimiting(RateLimitLevel.Low)]
    [ProducesResponseType(
        typeof(ApiResponse<PagedResult<MemberResponse>>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(
        [FromQuery] PagedRequest<MemberFilter> request,
        CancellationToken cancellationToken)
    {
        var result = await _memberService.SearchAsync(
            request,
            cancellationToken);

        return Ok(
            ApiResponse<PagedResult<MemberResponse>>.Ok(
                result,
				"Members retrieved successfully."));
    }

    [HttpPost]
    [EnableRateLimiting(RateLimitLevel.High)]
	[ProducesResponseType(
        typeof(ApiResponse<MemberResponse>),
        StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(
        [FromBody] CreateMemberRequest request,
        CancellationToken cancellationToken)
    {
        var member = await _memberService.CreateAsync(
            request,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = member.Id },
            ApiResponse<MemberResponse>.Ok(
                member,
				"Member created successfully."));
    }

    [HttpPut("{id:guid}")]
    [EnableRateLimiting(RateLimitLevel.High)]
	[ProducesResponseType(
        typeof(ApiResponse<MemberResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateMemberRequest request,
        CancellationToken cancellationToken)
    {
        var member = await _memberService.UpdateAsync(
            id,
            request,
            cancellationToken);

        return Ok(
            ApiResponse<MemberResponse>.Ok(
                member,
				"Member updated successfully."));
    }

    [HttpDelete("{id:guid}")]
    [EnableRateLimiting(RateLimitLevel.Moderate)]
	[ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _memberService.DeleteAsync(
            id,
            cancellationToken);

        return Ok(
            ApiResponse<object>.Ok(
                null,
				"Member deleted successfully."));
    }
}
