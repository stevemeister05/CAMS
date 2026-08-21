using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Application.Common.Exceptions;

public sealed class NotFoundException : CamsApplicationException
{
	public NotFoundException(
		string message,
		string code = "NOT_FOUND")
		: base(
			code,
			message,
			StatusCodes.Status404NotFound)
	{
	}
}
