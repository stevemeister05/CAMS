using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Application.Common.Exceptions;

public sealed class ConflictException : CamsApplicationException
{
	public ConflictException(
		string message,
		string code = "CONFLICT")
		: base(
			code,
			message,
			409)
	{
	}
}
