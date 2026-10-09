using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Application.Authentication;

public interface IAuthService
{
	Task<LoginResult> LoginAsync(LoginRequest request,
		CancellationToken cancellationToken = default);

	Task LogoutAsync();
}
