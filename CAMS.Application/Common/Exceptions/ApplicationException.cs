using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Application.Common.Exceptions;

public class CamsApplicationException : Exception
{
	public string Code { get; }

	public int StatusCode { get; }

	public CamsApplicationException(
		string code,
		string message,
		int statusCode)
		: base(message)
	{
		Code = code;
		StatusCode = statusCode;
	}

	public CamsApplicationException(
		string code,
		string message,
		int statusCode,
		Exception innerException)
		: base(message, innerException)
	{
		Code = code;
		StatusCode = statusCode;
	}
}
