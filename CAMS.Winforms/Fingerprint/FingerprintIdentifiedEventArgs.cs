namespace CAMS.Winforms.Fingerprint;

public sealed class FingerprintIdentifiedEventArgs
	: EventArgs
{
	public Guid MemberId
	{
		get;
	}


	public FingerprintIdentifiedEventArgs(
		Guid memberId)
	{
		MemberId =
			memberId;
	}
}