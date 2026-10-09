using Futronic.SDKHelper;

namespace CAMS.Winforms.Fingerprint;

public sealed class FutronicFingerprintEnroller
	: IFingerprintEnroller
{
	private readonly object
		_syncRoot =
			new();


	private EnrollmentContext?
		_context;


	private bool
		_disposed;


	public bool IsRunning
	{
		get
		{
			lock (_syncRoot)
			{
				return
					_context is not null;
			}
		}
	}


	public event EventHandler?
		FingerPlaced;


	public event EventHandler?
		FingerRemoved;


	public Task<FingerprintEnrollmentResult>
		EnrollAsync(
			CancellationToken cancellationToken = default)
	{
		ObjectDisposedException.ThrowIf(
			_disposed,
			this);


		cancellationToken
			.ThrowIfCancellationRequested();


		FutronicEnrollment
			operation;


		EnrollmentContext
			context;


		lock (_syncRoot)
		{
			if (_context is not null)
			{
				throw new InvalidOperationException(
					"Fingerprint enrollment is already running.");
			}


			operation =
				new FutronicEnrollment();


			context =
				new EnrollmentContext(
					operation,
					cancellationToken);


			_context =
				context;
		}


		/*
		 * Futronic's constructor already initializes sensible
		 * defaults:
		 *
		 * FakeDetection = false
		 * FFDControl = true
		 * FARnLevel = normal
		 * Version = current
		 * FastMode = false
		 *
		 * So we don't need to override those yet.
		 */


		operation.OnPutOn +=
			_ =>
			{
				FingerPlaced?.Invoke(
					this,
					EventArgs.Empty);
			};


		operation.OnTakeOff +=
			_ =>
			{
				FingerRemoved?.Invoke(
					this,
					EventArgs.Empty);
			};


		operation.OnFakeSource +=
			_ =>
			{
				/*
				 * true = cancel the current operation.
				 *
				 * FakeDetection is currently disabled,
				 * so this should not normally fire.
				 */
				return
					true;
			};


		operation.OnEnrollmentComplete +=
			(
				success,
				returnCode) =>
			{
				HandleEnrollmentCompleted(
					context,
					success,
					returnCode);
			};


		context.CancellationRegistration =
			cancellationToken.Register(
				() =>
				{
					try
					{
						operation
							.OnCalcel();
					}
					catch
					{
						// Ignore cancellation cleanup errors.
					}
				});


		try
		{
			operation
				.Enrollment();
		}
		catch (Exception exception)
		{
			CompleteWithException(
				context,
				exception);
		}


		return
			context.CompletionSource.Task;
	}


	public void Cancel()
	{
		EnrollmentContext?
			context;


		lock (_syncRoot)
		{
			context =
				_context;
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
			// Ignore errors while cancelling.
		}
	}


	public void Dispose()
	{
		if (_disposed)
		{
			return;
		}


		_disposed =
			true;


		EnrollmentContext?
			context;


		lock (_syncRoot)
		{
			context =
				_context;

			_context =
				null;
		}


		if (context is not null)
		{
			try
			{
				context.Operation
					.OnCalcel();
			}
			catch
			{
			}


			QueueCleanup(
				context);
		}


		GC.SuppressFinalize(
			this);
	}


	private void HandleEnrollmentCompleted(
		EnrollmentContext context,
		bool success,
		int returnCode)
	{
		if (!IsCurrentContext(
			context))
		{
			QueueCleanup(
				context);

			return;
		}


		if (success)
		{
			var template =
				context.Operation
					.Template?
					.ToArray();


			if (
				template is null ||
				template.Length == 0
			)
			{
				CompleteWithException(
					context,
					new InvalidOperationException(
						"Futronic completed enrollment " +
						"but returned an empty template."));

				return;
			}


			var result =
				new FingerprintEnrollmentResult(
					template,
					context.Operation.Quality);


			ClearCurrentContext(
				context);


			context.CompletionSource
				.TrySetResult(
					result);


			QueueCleanup(
				context);

			return;
		}


		ClearCurrentContext(
			context);


		if (context.CancellationToken.IsCancellationRequested)
		{
			context.CompletionSource
				.TrySetCanceled(
					context.CancellationToken);
		}
		else
		{
			context.CompletionSource
				.TrySetException(
					new InvalidOperationException(
						FutronicSdkBase
							.SdkRetCode2Message(
								returnCode)));
		}


		QueueCleanup(
			context);
	}


	private void CompleteWithException(
		EnrollmentContext context,
		Exception exception)
	{
		ClearCurrentContext(
			context);


		context.CompletionSource
			.TrySetException(
				exception);


		QueueCleanup(
			context);
	}


	private bool IsCurrentContext(
		EnrollmentContext context)
	{
		lock (_syncRoot)
		{
			return
				ReferenceEquals(
					_context,
					context);
		}
	}


	private void ClearCurrentContext(
		EnrollmentContext context)
	{
		lock (_syncRoot)
		{
			if (
				ReferenceEquals(
					_context,
					context)
			)
			{
				_context =
					null;
			}
		}
	}


	private static void QueueCleanup(
		EnrollmentContext context)
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
				() =>
				{
					try
					{
						context.CancellationRegistration
							.Dispose();


						context.Operation
							.Dispose();
					}
					catch
					{
						// Nothing else to do during cleanup.
					}
				});
	}


	private sealed class EnrollmentContext
	{
		public FutronicEnrollment Operation
		{
			get;
		}


		public CancellationToken CancellationToken
		{
			get;
		}


		public TaskCompletionSource<FingerprintEnrollmentResult>
			CompletionSource
		{
			get;
		}


		public CancellationTokenRegistration
			CancellationRegistration;


		public int
			CleanupStarted;


		public EnrollmentContext(
			FutronicEnrollment operation,
			CancellationToken cancellationToken)
		{
			Operation =
				operation;


			CancellationToken =
				cancellationToken;


			CompletionSource =
				new TaskCompletionSource<FingerprintEnrollmentResult>(
					TaskCreationOptions.RunContinuationsAsynchronously);
		}
	}
}