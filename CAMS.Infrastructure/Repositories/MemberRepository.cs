using CAMS.Application.Member;
using CAMS.Domain.Entities;
using CAMS.Infrastructure.Data;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;

namespace CAMS.Infrastructure.Repositories;

public class MemberRepository
	: Repository<Member>,
	  IMemberRepository
{
	public MemberRepository(CAMSDBContext context)
		: base(context)
	{
	}

	public async Task<Member?> GetByMobileNumberAsync(
		string mobileNumber,
		CancellationToken cancellationToken = default)
	{
		return await DbSet
			.FirstOrDefaultAsync(
				x => x.MobileNumber == mobileNumber,
				cancellationToken);
	}

	public async Task<IReadOnlyList<MemberFingerprint>> GetActiveFingerprintsAsync(
		CancellationToken cancellationToken = default)
	{
		return
			await Context
				.Set<MemberFingerprint>()
				.AsNoTracking()
				.Include(
					fingerprint =>
						fingerprint.Member)
				.Where(
					fingerprint =>
						fingerprint.IsActive &&
						fingerprint.Member.IsActive)
				.ToListAsync(
					cancellationToken);
	}

	public async Task AddFingerprintAsync(
		MemberFingerprint fingerprint,
		CancellationToken cancellationToken = default)
	{
		await Context
			.Set<MemberFingerprint>()
			.AddAsync(
				fingerprint,
				cancellationToken);
	}

	public async Task<MemberFingerprint?> GetActiveFingerprintByMemberAsync(
		Guid memberId,
		string? fingerLabel,
		CancellationToken cancellationToken = default)
	{
		var query =
			Context
				.Set<MemberFingerprint>()
				.Where(
					fingerprint =>
						fingerprint.MemberId ==
							memberId &&
						fingerprint.IsActive);


		if (!string.IsNullOrWhiteSpace(
			fingerLabel))
		{
			query =
				query.Where(
					fingerprint =>
						fingerprint.FingerLabel ==
							fingerLabel);
		}


		return
			await query
				.FirstOrDefaultAsync(
					cancellationToken);
	}
}
