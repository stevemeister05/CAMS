using CAMS.Application.Registration;
using CAMS.Domain.Entities;
using CAMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CAMS.Infrastructure.Repositories;

public class RegistrationRequestRepository
	: Repository<RegistrationRequest>,
	  IRegistrationRequestRepository
{
	public RegistrationRequestRepository(
		CAMSDBContext context)
		: base(context)
	{
	}

	public async Task<RegistrationRequest?> GetByMobileNumberAsync(
		string mobileNumber,
		CancellationToken cancellationToken = default)
	{
		return await DbSet
			.FirstOrDefaultAsync(
				x => x.MobileNumber == mobileNumber,
				cancellationToken);
	}
}