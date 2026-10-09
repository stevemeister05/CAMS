namespace CAMS.Winforms.Fingerprint;

public sealed class FingerprintReference
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