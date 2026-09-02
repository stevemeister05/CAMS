namespace CAMS.Application.Common.Exceptions;

public sealed class UnauthorizedException : CamsApplicationException
{
	public UnauthorizedException(
		string message,
		string code = "UNAUTHORIZED")
		: base(
			code,
			message,
			401)
	{
	}
}