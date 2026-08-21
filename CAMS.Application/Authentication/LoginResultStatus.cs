using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Application.Authentication;

public enum LoginResultStatus
{
	Success,

	InvalidCredentials,

	LockedOut,

	NotAllowed,

	RequiresTwoFactor
}