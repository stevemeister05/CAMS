using CAMS.Application.Common.Pagination;
using CAMS.Application.Common.Repositories;
using CAMS.Application.Member.DTOs;
using CAMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Application.Member;

public interface IMemberRepository : IRepository<CAMS.Domain.Entities.Member>
{
	Task<CAMS.Domain.Entities.Member?> GetByMobileNumberAsync(
		string mobileNumber,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<MemberFingerprint>> GetActiveFingerprintsAsync(
		CancellationToken cancellationToken = default);

	Task AddFingerprintAsync(MemberFingerprint fingerprint, CancellationToken cancellationToken = default);

	Task<MemberFingerprint?> GetActiveFingerprintByMemberAsync(
		Guid memberId,
		string? fingerLabel,
		CancellationToken cancellationToken = default);
}
