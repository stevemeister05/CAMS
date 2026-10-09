namespace CAMS.Winforms.Fingerprint;

public sealed class FingerprintScanFailedEventArgs
	: EventArgs
{
	public string Message
	{
		get;
	}


	public int? ReturnCode
	{
		get;
	}


	public FingerprintScanFailedEventArgs(
		string message,
		int? returnCode = null)
	{
		Message =
			message;

		ReturnCode =
			returnCode;
	}
}