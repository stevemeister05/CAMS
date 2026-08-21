using CAMS.Application.Common.Repositories;
using CAMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Application.Registration;

public interface IRegistrationRequestRepository
	: IRepository<RegistrationRequest>
{
	Task<RegistrationRequest?> GetByMobileNumberAsync(
		string mobileNumber,
		CancellationToken cancellationToken = default);
}
