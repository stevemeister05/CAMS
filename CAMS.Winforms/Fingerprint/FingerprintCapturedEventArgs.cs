namespace CAMS.Winforms.Fingerprint;

public sealed class FingerprintCapturedEventArgs
	: EventArgs
{
	public byte[] Template { get; }


	public FingerprintCapturedEventArgs(
		byte[] template)
	{
		Template =
			template;
	}
}