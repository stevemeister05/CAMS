namespace CAMS.Winforms.Fingerprint;

public interface IFingerprintScanner
	: IDisposable
{
	bool IsConnected
	{
		get;
	}


	bool IsRunning
	{
		get;
	}


	event EventHandler?
		Connected;


	event EventHandler?
		Disconnected;


	event EventHandler?
		FingerPlaced;


	event EventHandler?
		FingerRemoved;


	event EventHandler<FingerprintIdentifiedEventArgs>?
		FingerprintIdentified;


	event EventHandler?
		FingerprintNotRecognized;


	event EventHandler<FingerprintScanFailedEventArgs>?
		ScanFailed;


	/*
	 * Temporary legacy event.
	 *
	 * We will remove this once AttendanceControl
	 * has been converted to identification.
	 */
	event EventHandler<FingerprintCapturedEventArgs>?
		FingerprintCaptured;


	Task StartIdentificationAsync(
		IReadOnlyList<FingerprintReference> references,
		CancellationToken cancellationToken = default);


	void Stop();
}