using Futronic.SDKHelper;

namespace CAMS.Winforms.Fingerprint;

public sealed class FutronicFingerprintScanner
	: IFingerprintScanner
{
	private readonly object
		_syncRoot =
			new();


	private OperationContext?
		_operationContext;


	private CancellationTokenRegistration
		_cancellationRegistration;


	private bool
		_isRunning;


	private bool
		_isConnected;


	private bool
		_disposed;


	private long
		_sessionId;


	public bool IsConnected
	{
		get
		{
			lock (_syncRoot)
			{
				return
					_isConnected;
			}
		}
	}


	public bool IsRunning
	{
		get
		{
			lock (_syncRoot)
			{
				return
					_isRunning;
			}
		}
	}


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
		ObjectDisposedException
			.ThrowIf(
				_disposed,
				this);


		ArgumentNullException
			.ThrowIfNull(
				references);


		cancellationToken
			.ThrowIfCancellationRequested();


		var validReferences =
			references
				.Where(
					reference =>
						reference.MemberId != Guid.Empty &&
						reference.Template.Length > 0)
				.Select(
					reference =>
						new FingerprintReference
						{
							MemberId =
								reference.MemberId,

							Template =
								reference.Template
									.ToArray()
						})
				.ToArray();


		if (validReferences.Length == 0)
		{
			throw new InvalidOperationException(
				"No enrolled fingerprint templates are available.");
		}


		long sessionId;


		lock (_syncRoot)
		{
			if (_isRunning)
			{
				throw new InvalidOperationException(
					"The fingerprint scanner is already running.");
			}


			_isRunning =
				true;


			sessionId =
				++_sessionId;
		}


		_cancellationRegistration
			.Dispose();


		_cancellationRegistration =
			cancellationToken.Register(
				Stop);


		StartCaptureCycle(
			sessionId,
			validReferences);


		return
			Task.CompletedTask;
	}


	public void Stop()
	{
		OperationContext?
			context;


		lock (_syncRoot)
		{
			if (!_isRunning)
			{
				return;
			}


			_isRunning =
				false;


			_sessionId++;


			context =
				_operationContext;


			_operationContext =
				null;
		}


		if (context is null)
		{
			return;
		}


		try
		{
			context.Operation
				.OnCalcel();
		}
		catch
		{
			// Ignore errors while stopping.
		}


		QueueCleanup(
			context,
			restart: false);
	}


	public void Dispose()
	{
		if (_disposed)
		{
			return;
		}


		_disposed =
			true;


		Stop();


		_cancellationRegistration
			.Dispose();


		GC.SuppressFinalize(
			this);
	}


	private void StartCaptureCycle(
		long sessionId,
		IReadOnlyList<FingerprintReference> references)
	{
		if (!IsSessionActive(
			sessionId))
		{
			return;
		}


		FutronicIdentification
			operation;


		try
		{
			operation =
				new FutronicIdentification();


			ConfigureOperation(
				operation);
		}
		catch (Exception exception)
		{
			SetConnected(
				false);


			RaiseScanFailed(
				exception.Message);


			Stop();

			return;
		}


		var context =
			new OperationContext(
				sessionId,
				operation,
				references);


		operation.OnPutOn +=
			_ =>
			{
				if (!IsSessionActive(
					sessionId))
				{
					return;
				}


				SetConnected(
					true);


				FingerPlaced?.Invoke(
					this,
					EventArgs.Empty);
			};


		operation.OnTakeOff +=
			_ =>
			{
				if (!IsSessionActive(
					sessionId))
				{
					return;
				}


				FingerRemoved?.Invoke(
					this,
					EventArgs.Empty);
			};


		operation.OnFakeSource +=
			_ =>
			{
				/*
				 * Returning true tells Futronic
				 * to cancel the current operation.
				 */
				return
					true;
			};


		operation.OnGetBaseTemplateComplete +=
			(
				success,
				returnCode) =>
			{
				HandleCaptureCompleted(
					context,
					success,
					returnCode);
			};


		lock (_syncRoot)
		{
			if (
				!_isRunning ||
				_sessionId != sessionId
			)
			{
				QueueCleanup(
					context,
					restart: false);

				return;
			}


			_operationContext =
				context;
		}


		try
		{
			operation
				.GetBaseTemplate();
		}
		catch (Exception exception)
		{
			RaiseScanFailed(
				exception.Message);


			QueueCleanup(
				context,
				restart: true);
		}
	}


	private void ConfigureOperation(
		FutronicIdentification operation)
	{
		/*
		 * Start with conservative SDK defaults.
		 *
		 * We can expose these as CAMS settings later.
		 */
		operation.FakeDetection =
			false;

		operation.FFDControl =
			true;

		operation.FastMode =
			false;

		operation.FARnLevel =
			FarnValues.farn_normal;

		operation.Version =
			VersionCompatible.ftr_version_current;
	}


	private void HandleCaptureCompleted(
		OperationContext context,
		bool success,
		int returnCode)
	{
		if (!IsSessionActive(
			context.SessionId))
		{
			QueueCleanup(
				context,
				restart: false);

			return;
		}


		if (!success)
		{
			HandleCaptureFailure(
				context,
				returnCode);

			return;
		}


		SetConnected(
			true);


		try
		{
			IdentifyMember(
				context);
		}
		catch (Exception exception)
		{
			RaiseScanFailed(
				exception.Message);
		}
		finally
		{
			QueueCleanup(
				context,
				restart: true);
		}
	}


	private void HandleCaptureFailure(
		OperationContext context,
		int returnCode)
	{
		if (
			returnCode ==
			FutronicSdkBase.RETCODE_DEVICE_NOT_CONNECTED
		)
		{
			SetConnected(
				false);


			RaiseScanFailed(
				FutronicSdkBase
					.SdkRetCode2Message(
						returnCode),
				returnCode);


			/*
			 * Don't continuously retry when
			 * the scanner isn't connected.
			 */
			lock (_syncRoot)
			{
				_isRunning =
					false;
			}


			QueueCleanup(
				context,
				restart: false);

			return;
		}


		if (
			returnCode ==
			FutronicSdkBase.RETCODE_CANCELED_BY_USER
		)
		{
			QueueCleanup(
				context,
				restart: IsSessionActive(
					context.SessionId));

			return;
		}


		RaiseScanFailed(
			FutronicSdkBase
				.SdkRetCode2Message(
					returnCode),
			returnCode);


		QueueCleanup(
			context,
			restart: true);
	}


	private void IdentifyMember(
		OperationContext context)
	{
		var records =
			context.References
				.Select(
					reference =>
						new FtrIdentifyRecord
						{
							KeyValue =
								reference.MemberId
									.ToByteArray(),

							Template =
								reference.Template
						})
				.ToArray();


		var matchIndex =
			records.Length;


		var returnCode =
			context.Operation
				.Identification(
					records,
					ref matchIndex);


		if (
			returnCode !=
			FutronicSdkBase.RETCODE_OK
		)
		{
			RaiseScanFailed(
				FutronicSdkBase
					.SdkRetCode2Message(
						returnCode),
				returnCode);

			return;
		}


		if (
			matchIndex < 0 ||
			matchIndex >=
				context.References.Count
		)
		{
			FingerprintNotRecognized?.Invoke(
				this,
				EventArgs.Empty);

			return;
		}


		var memberId =
			context.References[
				matchIndex
			].MemberId;


		FingerprintIdentified?.Invoke(
			this,
			new FingerprintIdentifiedEventArgs(
				memberId));
	}


	private void QueueCleanup(
		OperationContext context,
		bool restart)
	{
		if (
			Interlocked.Exchange(
				ref context.CleanupStarted,
				1) != 0
		)
		{
			return;
		}


		_ =
			Task.Run(
				async () =>
				{
					try
					{
						/*
						 * Dispose from another thread rather
						 * than from Futronic's completion callback.
						 */
						context.Operation
							.Dispose();
					}
					catch
					{
						// Ignore cleanup failures.
					}


					var shouldRestart =
						false;


					lock (_syncRoot)
					{
						if (
							ReferenceEquals(
								_operationContext,
								context)
						)
						{
							_operationContext =
								null;
						}


						shouldRestart =
							restart &&
							_isRunning &&
							_sessionId ==
								context.SessionId;
					}


					if (!shouldRestart)
					{
						return;
					}


					/*
					 * Small pause before arming the
					 * scanner for the next member.
					 */
					await Task.Delay(
						250);


					if (!IsSessionActive(
						context.SessionId))
					{
						return;
					}


					StartCaptureCycle(
						context.SessionId,
						context.References);
				});
	}


	private bool IsSessionActive(
		long sessionId)
	{
		lock (_syncRoot)
		{
			return
				!_disposed &&
				_isRunning &&
				_sessionId ==
					sessionId;
		}
	}


	private void SetConnected(
		bool connected)
	{
		var changed =
			false;


		lock (_syncRoot)
		{
			if (
				_isConnected ==
				connected
			)
			{
				return;
			}


			_isConnected =
				connected;

			changed =
				true;
		}


		if (!changed)
		{
			return;
		}


		if (connected)
		{
			Connected?.Invoke(
				this,
				EventArgs.Empty);
		}
		else
		{
			Disconnected?.Invoke(
				this,
				EventArgs.Empty);
		}
	}


	private void RaiseScanFailed(
		string message,
		int? returnCode = null)
	{
		ScanFailed?.Invoke(
			this,
			new FingerprintScanFailedEventArgs(
				message,
				returnCode));
	}


	private sealed class OperationContext
	{
		public long SessionId
		{
			get;
		}


		public FutronicIdentification Operation
		{
			get;
		}


		public IReadOnlyList<FingerprintReference>
			References
		{
			get;
		}


		public int CleanupStarted;


		public OperationContext(
			long sessionId,
			FutronicIdentification operation,
			IReadOnlyList<FingerprintReference> references)
		{
			SessionId =
				sessionId;

			Operation =
				operation;

			References =
				references;
		}
	}
}