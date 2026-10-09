using CAMS.Application.Common.Pagination;
using CAMS.Application.Member.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Application.Member;

public interface IMemberService
{
	Task<MemberResponse> GetByIdAsync(
		Guid id,
		CancellationToken cancellationToken = default);

	Task<PagedResult<MemberResponse>> SearchAsync(
		PagedRequest<MemberFilter> request,
		CancellationToken cancellationToken = default);

	Task<MemberResponse> CreateAsync(
		CreateMemberRequest request,
		CancellationToken cancellationToken = default);

	Task<MemberResponse?> UpdateAsync(
		Guid id,
		UpdateMemberRequest request,
		CancellationToken cancellationToken = default);

	Task<bool> DeleteAsync(
		Guid id,
		CancellationToken cancellationToken = default);

	Task<MemberResponse> ActivateAsync(
		Guid id,
		CancellationToken cancellationToken = default);

	Task<MemberResponse> DeactivateAsync(
		Guid id,
		CancellationToken cancellationToken = default);

	Task<MemberFingerprintResponse> EnrollFingerprintAsync(
		Guid memberId,
		EnrollFingerprintRequest request,
		CancellationToken cancellationToken = default);
}
