namespace CAMS.Application.Attendance.DTOs;

public sealed class FingerprintReferenceResponse
{
	public Guid MemberId
	{
		get;
		init;
	}


	public byte[] Template
	{
		get;
		init;
	} = [];
}