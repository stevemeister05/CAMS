using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Application.Common.Exceptions;

public sealed class ValidationException : CamsApplicationException
{
	public ValidationException(
		string message,
		string code = "VALIDATION_ERROR")
		: base(
			code,
			message,
			400)
	{
	}
}
