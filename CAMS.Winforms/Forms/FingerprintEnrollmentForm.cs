using CAMS.Application.Member.DTOs;
using CAMS.Winforms.Fingerprint;
using CAMS.Winforms.Services;

namespace CAMS.Winforms.Forms;

public partial class FingerprintEnrollmentForm
	: Form
{
	private readonly Guid
		_memberId;


	private readonly string
		_memberName;


	private readonly MemberApiService
		_memberService;


	private readonly IFingerprintEnroller
		_fingerprintEnroller;


	private CancellationTokenSource?
		_cancellationTokenSource;


	private bool
		_isProcessing;


	private bool
		_closeAfterCancellation;


	public MemberFingerprintResponse?
		EnrolledFingerprint
	{
		get;
		private set;
	}


	public FingerprintEnrollmentForm(
		Guid memberId,
		string memberName,
		MemberApiService memberService,
		IFingerprintEnroller fingerprintEnroller)
	{
		InitializeComponent();


		if (memberId == Guid.Empty)
		{
			throw new ArgumentException(
				"Member ID is required.",
				nameof(memberId));
		}


		_memberId =
			memberId;


		_memberName =
			memberName;


		_memberService =
			memberService;


		_fingerprintEnroller =
			fingerprintEnroller;


		InitializePage();
	}


	private void InitializePage()
	{
		lblMemberValue.Text =
			_memberName;


		cmbFinger.Items.AddRange(
			[
				"Right Thumb",
				"Right Index",
				"Right Middle",
				"Right Ring",
				"Right Little",
				"Left Thumb",
				"Left Index",
				"Left Middle",
				"Left Ring",
				"Left Little"
			]);


		cmbFinger.SelectedIndex =
			0;


		lblStatus.Text =
			"Ready to enroll.";


		lblQuality.Text =
			"—";


		prgEnrollment.Visible =
			false;


		_fingerprintEnroller.FingerPlaced +=
			FingerprintEnroller_FingerPlaced;


		_fingerprintEnroller.FingerRemoved +=
			FingerprintEnroller_FingerRemoved;


		btnStart.Click +=
			BtnStart_Click;


		btnCancel.Click +=
			BtnCancel_Click;


		FormClosing +=
			FingerprintEnrollmentForm_FormClosing;
	}


	private async void BtnStart_Click(
		object? sender,
		EventArgs e)
	{
		await StartEnrollmentAsync();
	}


	private async Task StartEnrollmentAsync()
	{
		if (_isProcessing)
		{
			return;
		}


		if (
			cmbFinger.SelectedItem is not string
				fingerLabel ||
			string.IsNullOrWhiteSpace(
				fingerLabel)
		)
		{
			MessageBox.Show(
				this,
				"Please select a finger.",
				"Fingerprint Enrollment",
				MessageBoxButtons.OK,
				MessageBoxIcon.Warning);

			return;
		}


		_isProcessing =
			true;


		_closeAfterCancellation =
			false;


		_cancellationTokenSource?.Dispose();


		_cancellationTokenSource =
			new CancellationTokenSource();


		SetEnrollmentControls(
			running: true);


		lblQuality.Text =
			"—";


		SetStatus(
			$"Preparing to enroll {fingerLabel}...");


		try
		{
			var enrollmentResult =
				await _fingerprintEnroller
					.EnrollAsync(
						_cancellationTokenSource.Token);


			if (
				_cancellationTokenSource
					.IsCancellationRequested
			)
			{
				return;
			}


			lblQuality.Text =
				enrollmentResult.Quality
					.ToString();


			SetStatus(
				"Fingerprint captured successfully. Saving...");


			EnrolledFingerprint =
				await _memberService
					.EnrollFingerprintAsync(
						_memberId,
						enrollmentResult.Template,
						fingerLabel,
						_cancellationTokenSource.Token);


			SetStatus(
				"Fingerprint enrolled successfully.");


			MessageBox.Show(
				this,
				$"{fingerLabel} was enrolled successfully " +
				$"for {_memberName}.\n\n" +
				$"Quality: {enrollmentResult.Quality}",
				"Fingerprint Enrollment",
				MessageBoxButtons.OK,
				MessageBoxIcon.Information);


			DialogResult =
				DialogResult.OK;


			Close();
		}
		catch (OperationCanceledException)
		{
			SetStatus(
				"Fingerprint enrollment cancelled.");
		}
		catch (Exception exception)
		{
			SetStatus(
				"Fingerprint enrollment failed.");


			MessageBox.Show(
				this,
				exception.Message,
				"Fingerprint Enrollment",
				MessageBoxButtons.OK,
				MessageBoxIcon.Error);
		}
		finally
		{
			_isProcessing =
				false;


			SetEnrollmentControls(
				running: false);


			if (_closeAfterCancellation)
			{
				_closeAfterCancellation =
					false;


				BeginInvoke(
					Close);
			}
		}
	}


	private void FingerprintEnroller_FingerPlaced(
		object? sender,
		EventArgs e)
	{
		RunOnUiThread(
			() =>
			{
				var finger =
					cmbFinger.SelectedItem?
						.ToString() ??
					"finger";


				SetStatus(
					$"Place your {finger.ToLowerInvariant()} " +
					"on the scanner.");
			});
	}


	private void FingerprintEnroller_FingerRemoved(
		object? sender,
		EventArgs e)
	{
		RunOnUiThread(
			() =>
			{
				SetStatus(
					"Remove your finger from the scanner.");
			});
	}


	private void BtnCancel_Click(
		object? sender,
		EventArgs e)
	{
		if (!_isProcessing)
		{
			DialogResult =
				DialogResult.Cancel;


			Close();

			return;
		}


		CancelEnrollment();
	}


	private void FingerprintEnrollmentForm_FormClosing(
		object? sender,
		FormClosingEventArgs e)
	{
		if (!_isProcessing)
		{
			UnsubscribeEvents();

			return;
		}


		/*
		 * Don't dispose the form while Futronic is still
		 * completing its worker operation.
		 */
		e.Cancel =
			true;


		_closeAfterCancellation =
			true;


		CancelEnrollment();
	}


	private void CancelEnrollment()
	{
		if (!_isProcessing)
		{
			return;
		}


		SetStatus(
			"Cancelling fingerprint enrollment...");


		btnCancel.Enabled =
			false;


		try
		{
			_cancellationTokenSource?
				.Cancel();


			_fingerprintEnroller
				.Cancel();
		}
		catch
		{
			// Cancellation is best-effort.
		}
	}


	private void SetEnrollmentControls(
		bool running)
	{
		cmbFinger.Enabled =
			!running;


		btnStart.Enabled =
			!running;


		btnStart.Text =
			running
				? "Enrolling..."
				: "Start Enrollment";


		btnCancel.Enabled =
			true;


		btnCancel.Text =
			running
				? "Cancel Enrollment"
				: "Cancel";


		prgEnrollment.Visible =
			running;


		prgEnrollment.Style =
			ProgressBarStyle.Marquee;
	}


	private void SetStatus(
		string message)
	{
		lblStatus.Text =
			message;
	}


	private void RunOnUiThread(
		Action action)
	{
		if (
			IsDisposed ||
			Disposing
		)
		{
			return;
		}


		try
		{
			if (InvokeRequired)
			{
				BeginInvoke(
					action);

				return;
			}


			action();
		}
		catch (
			InvalidOperationException)
		{
			/*
			 * The form may have been destroyed
			 * while Futronic was finishing a callback.
			 */
		}
	}


	private void UnsubscribeEvents()
	{
		_fingerprintEnroller.FingerPlaced -=
			FingerprintEnroller_FingerPlaced;


		_fingerprintEnroller.FingerRemoved -=
			FingerprintEnroller_FingerRemoved;


		_cancellationTokenSource?
			.Dispose();


		_cancellationTokenSource =
			null;
	}
}