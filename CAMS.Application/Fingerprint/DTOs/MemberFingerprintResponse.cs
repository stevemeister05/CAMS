namespace CAMS.Application.Member.DTOs;

public sealed class MemberFingerprintResponse
{
	public Guid Id
	{
		get;
		init;
	}


	public Guid MemberId
	{
		get;
		init;
	}


	public string? FingerLabel
	{
		get;
		init;
	}


	public bool IsActive
	{
		get;
		init;
	}
}