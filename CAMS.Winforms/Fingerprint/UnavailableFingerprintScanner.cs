namespace CAMS.Winforms.Fingerprint;

public sealed class UnavailableFingerprintScanner
	: IFingerprintScanner
{
	public bool IsConnected =>
		false;


	public bool IsRunning =>
		false;


	public event EventHandler?
		Connected;


	public event EventHandler?
		Disconnected;


	public event EventHandler?
		FingerPlaced;


	public event EventHandler?
		FingerRemoved;


	public event EventHandler<FingerprintIdentifiedEventArgs>?
		FingerprintIdentified;


	public event EventHandler?
		FingerprintNotRecognized;


	public event EventHandler<FingerprintScanFailedEventArgs>?
		ScanFailed;


	public event EventHandler<FingerprintCapturedEventArgs>?
		FingerprintCaptured;


	public Task StartIdentificationAsync(
		IReadOnlyList<FingerprintReference> references,
		CancellationToken cancellationToken = default)
	{
		return
			Task.CompletedTask;
	}


	public void Stop()
	{
	}


	public void Dispose()
	{
	}
}