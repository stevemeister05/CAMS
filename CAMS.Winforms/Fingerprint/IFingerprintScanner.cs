namespace CAMS.Winforms.Fingerprint;

public interface IFingerprintScanner
	: IDisposable
{
	bool IsConnected { get; }


	event EventHandler?
		Connected;

	event EventHandler?
		Disconnected;

	event EventHandler<FingerprintCapturedEventArgs>?
		FingerprintCaptured;


	Task StartAsync(
		CancellationToken cancellationToken = default);


	void Stop();
}