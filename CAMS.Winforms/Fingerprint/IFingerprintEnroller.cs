namespace CAMS.Winforms.Fingerprint;

public interface IFingerprintEnroller
	: IDisposable
{
	bool IsRunning
	{
		get;
	}


	event EventHandler?
		FingerPlaced;


	event EventHandler?
		FingerRemoved;


	Task<FingerprintEnrollmentResult>
		EnrollAsync(
			CancellationToken cancellationToken = default);


	void Cancel();
}