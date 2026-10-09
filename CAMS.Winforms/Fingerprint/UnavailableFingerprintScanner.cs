namespace CAMS.Winforms.Fingerprint;

public sealed class UnavailableFingerprintScanner
	: IFingerprintScanner
{
	public bool IsConnected =>
		false;


	public event EventHandler?
		Connected;

	public event EventHandler?
		Disconnected;

	public event EventHandler<FingerprintCapturedEventArgs>?
		FingerprintCaptured;


	public Task StartAsync(
		CancellationToken cancellationToken = default)
	{
		return Task.CompletedTask;
	}


	public void Stop()
	{
	}


	public void Dispose()
	{
	}
}