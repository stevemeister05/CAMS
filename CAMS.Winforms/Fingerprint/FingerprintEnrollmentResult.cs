namespace CAMS.Winforms.Fingerprint;

public sealed class FingerprintEnrollmentResult
{
	public byte[] Template
	{
		get;
	}


	public uint Quality
	{
		get;
	}


	public FingerprintEnrollmentResult(
		byte[] template,
		uint quality)
	{
		Template =
			template;

		Quality =
			quality;
	}
}