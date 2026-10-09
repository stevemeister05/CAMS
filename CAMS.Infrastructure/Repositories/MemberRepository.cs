using CAMS.Application.Common.Pagination;
using CAMS.Application.Member;
using CAMS.Domain.Entities;
using CAMS.Infrastructure.Data;
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
}
