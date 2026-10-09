namespace CAMS.Application.Member.DTOs;

public sealed class EnrollFingerprintRequest
{
	public byte[] Template
	{
		get;
		init;
	} = [];


	public string? FingerLabel
	{
		get;
		init;
	}
}