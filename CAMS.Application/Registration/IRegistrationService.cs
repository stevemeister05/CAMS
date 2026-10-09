using CAMS.Application.Common.Pagination;
using CAMS.Application.Registration.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Application.Registration;

public interface IRegistrationService
{
	Task<RegistrationResponse> RegisterAsync(
		RegisterRequest request,
		CancellationToken cancellationToken = default);

	Task<RegistrationResponse> GetByIdAsync(
		Guid id,
		CancellationToken cancellationToken = default);

	Task<PagedResult<RegistrationResponse>> SearchAsync(
		PagedRequest<RegistrationFilter> request,
		CancellationToken cancellationToken = default);

	Task<RegistrationResponse> ApproveAsync(
		Guid id,
		CancellationToken cancellationToken = default);

	Task RejectAsync(
		Guid id,
		RejectRegistrationRequest request,
		CancellationToken cancellationToken = default);
}
