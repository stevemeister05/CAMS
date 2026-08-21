using CAMS.Application.Common.Pagination;
using CAMS.Application.Common.Repositories;
using CAMS.Application.Member.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Application.Member;

public interface IMemberRepository : IRepository<CAMS.Domain.Entities.Member>
{
	Task<CAMS.Domain.Entities.Member?> GetByMobileNumberAsync(
		string mobileNumber,
		CancellationToken cancellationToken = default);
}
