using CAMS.Application.Attendance.DTOs;
using CAMS.Domain.Enums;
using CAMS.Winforms.Fingerprint;
using CAMS.Winforms.Services;

namespace CAMS.Winforms.Controls;

public partial class AttendanceControl
	: UserControl
{
	private readonly Guid
		_eventId;

	private readonly AttendanceApiService?
		_attendanceService;

	private readonly IFingerprintScanner?
		_fingerprintScanner;


	private AttendancePageResponse?
		_attendancePage;


	private AttendanceAction
		_selectedAction =
			AttendanceAction.TimeIn;


	private CancellationTokenSource?
		_scannerCancellationTokenSource;


	private bool
		_processingFingerprint;


	public event EventHandler?
		BackRequested;


	public AttendanceControl()
	{
		InitializeComponent();

		InitializePage();
	}


	public AttendanceControl(
		Guid eventId,
		AttendanceApiService attendanceService,
		IFingerprintScanner fingerprintScanner)
		: this()
	{
		_eventId =
			eventId;

		_attendanceService =
			attendanceService;

		_fingerprintScanner =
			fingerprintScanner;
	}


	private void InitializePage()
	{
		Dock =
			DockStyle.Fill;


		ConfigureAttendanceGrid();


		lblScannerStatus.Text =
			"Scanner disconnected";

		lblScannerMessage.Text =
			"Waiting for scanner...";


		UpdateAttendanceMode();
	}


	private void ConfigureAttendanceGrid()
	{
		dgvAttendance.AutoGenerateColumns =
			false;

		dgvAttendance.ReadOnly =
			true;

		dgvAttendance.AllowUserToAddRows =
			false;

		dgvAttendance.AllowUserToDeleteRows =
			false;

		dgvAttendance.AllowUserToResizeRows =
			false;

		dgvAttendance.MultiSelect =
			false;

		dgvAttendance.SelectionMode =
			DataGridViewSelectionMode.FullRowSelect;

		dgvAttendance.RowHeadersVisible =
			false;
	}


	protected override async void OnLoad(
		EventArgs e)
	{
		base.OnLoad(
			e);


		if (DesignMode)
		{
			return;
		}


		await LoadAttendanceAsync();


		await StartFingerprintScannerAsync();
	}


	private async Task LoadAttendanceAsync()
	{
		if (
			_attendanceService is null ||
			_eventId ==
				Guid.Empty
		)
		{
			return;
		}


		SetLoading(
			true);


		try
		{
			_attendancePage =
				await _attendanceService
					.GetByEventAsync(
						_eventId);


			_selectedAction =
				_attendancePage.QrAction;


			RenderAttendancePage();
		}
		catch (HttpRequestException)
		{
			ShowError(
				"Unable to connect to the CAMS server.");
		}
		catch (TaskCanceledException)
		{
			ShowError(
				"The request to the CAMS server timed out.");
		}
		catch (Exception exception)
		{
			ShowError(
				exception.Message);
		}
		finally
		{
			SetLoading(
				false);
		}
	}


	private void RenderAttendancePage()
	{
		if (_attendancePage is null)
		{
			return;
		}


		var @event =
			_attendancePage.Event;


		if (@event is null)
		{
			ShowError(
				"Event information was not returned by the server.");

			return;
		}


		lblEventName.Text =
			@event.Name;


		lblEventDate.Text =
			@event.EventDate.ToString(
				"dddd, MMMM d, yyyy");


		lblTimeInWindow.Text =
			FormatWindow(
				@event.AttendanceTimeInStart,
				@event.AttendanceTimeInEnd);


		lblTimeOutWindow.Text =
			FormatWindow(
				@event.AttendanceTimeOutStart,
				@event.AttendanceTimeOutEnd);


		RenderAttendances();


		UpdateAttendanceMode();
	}


	private void RenderAttendances()
	{
		dgvAttendance.Rows.Clear();


		if (_attendancePage is null)
		{
			return;
		}


		var attendances =
			_attendancePage.Attendances ??
			[];


		lblAttendanceCount.Text =
			attendances.Count.ToString();


		if (attendances.Count == 0)
		{
			lblStatus.Text =
				"No attendance has been recorded yet.";

			lblStatus.Visible =
				true;

			return;
		}


		lblStatus.Visible =
			false;


		foreach (
			var attendance in
			attendances
		)
		{
			var rowIndex =
				dgvAttendance.Rows.Add();


			var row =
				dgvAttendance.Rows[
					rowIndex
				];


			row.Cells[
				colMember.Index
			].Value =
				attendance.MemberName;


			row.Cells[
				colTimeIn.Index
			].Value =
				FormatAttendanceTime(
					attendance.TimeIn);


			row.Cells[
				colTimeOut.Index
			].Value =
				FormatAttendanceTime(
					attendance.TimeOut);


			row.Tag =
				attendance;
		}
	}


	private async Task StartFingerprintScannerAsync()
	{
		if (_fingerprintScanner is null)
		{
			return;
		}


		_fingerprintScanner.Connected +=
			FingerprintScanner_Connected;

		_fingerprintScanner.Disconnected +=
			FingerprintScanner_Disconnected;

		_fingerprintScanner.FingerprintCaptured +=
			FingerprintScanner_FingerprintCaptured;


		_scannerCancellationTokenSource =
			new CancellationTokenSource();


		try
		{
			//await _fingerprintScanner
			//	.StartAsync(
			//		_scannerCancellationTokenSource.Token);


			UpdateScannerStatus();
		}
		catch (Exception exception)
		{
			lblScannerStatus.Text =
				"Scanner unavailable";

			lblScannerMessage.Text =
				exception.Message;
		}
	}


	private void FingerprintScanner_Connected(
		object? sender,
		EventArgs e)
	{
		if (InvokeRequired)
		{
			BeginInvoke(
				() =>
					FingerprintScanner_Connected(
						sender,
						e));

			return;
		}


		UpdateScannerStatus();
	}


	private void FingerprintScanner_Disconnected(
		object? sender,
		EventArgs e)
	{
		if (InvokeRequired)
		{
			BeginInvoke(
				() =>
					FingerprintScanner_Disconnected(
						sender,
						e));

			return;
		}


		UpdateScannerStatus();
	}


	private async void FingerprintScanner_FingerprintCaptured(
		object? sender,
		FingerprintCapturedEventArgs e)
	{
		if (_processingFingerprint)
		{
			return;
		}


		_processingFingerprint =
			true;


		try
		{
			SetScannerMessage(
				"Identifying fingerprint...");


			/*
			 * Later:
			 *
			 * 1. Match e.Template locally.
			 * 2. Resolve MemberId.
			 * 3. Call:
			 *
			 * POST /api/v1/attendance/fingerprint
			 *
			 * using:
			 *
			 * MemberId
			 * EventId
			 * _selectedAction
			 */


			await Task.CompletedTask;
		}
		catch (Exception exception)
		{
			SetScannerMessage(
				exception.Message);
		}
		finally
		{
			_processingFingerprint =
				false;


			if (
				_fingerprintScanner?.IsConnected ==
				true
			)
			{
				SetScannerMessage(
					"Ready for fingerprint...");
			}
		}
	}


	private void UpdateScannerStatus()
	{
		if (
			_fingerprintScanner?.IsConnected ==
			true
		)
		{
			lblScannerStatus.Text =
				"Scanner connected";

			lblScannerMessage.Text =
				"Ready for fingerprint...";

			return;
		}


		lblScannerStatus.Text =
			"Scanner disconnected";

		lblScannerMessage.Text =
			"Waiting for scanner...";
	}


	private void SetScannerMessage(
		string message)
	{
		if (InvokeRequired)
		{
			BeginInvoke(
				() =>
					SetScannerMessage(
						message));

			return;
		}


		lblScannerMessage.Text =
			message;
	}


	private void btnTimeIn_Click(
		object sender,
		EventArgs e)
	{
		_selectedAction =
			AttendanceAction.TimeIn;


		UpdateAttendanceMode();
	}


	private void btnTimeOut_Click(
		object sender,
		EventArgs e)
	{
		_selectedAction =
			AttendanceAction.TimeOut;


		UpdateAttendanceMode();
	}


	private void UpdateAttendanceMode()
	{
		var timeIn =
			_selectedAction ==
			AttendanceAction.TimeIn;


		btnTimeIn.Text =
			timeIn
				? "✓ Time In"
				: "Time In";


		btnTimeOut.Text =
			!timeIn
				? "✓ Time Out"
				: "Time Out";
	}


	private async void btnRefresh_Click(
		object sender,
		EventArgs e)
	{
		await LoadAttendanceAsync();
	}


	private void btnBack_Click(
		object sender,
		EventArgs e)
	{
		BackRequested?.Invoke(
			this,
			EventArgs.Empty);
	}


	private static string FormatWindow(
		TimeOnly start,
		TimeOnly end)
	{
		return
			start.ToString(
				"h:mm tt") +
			" - " +
			end.ToString(
				"h:mm tt");
	}


	private static string FormatAttendanceTime(
		DateTime? value)
	{
		if (!value.HasValue)
		{
			return "—";
		}


		return value.Value
			.ToLocalTime()
			.ToString(
				"h:mm tt");
	}


	private void SetLoading(
		bool loading)
	{
		btnRefresh.Enabled =
			!loading;

		btnBack.Enabled =
			!loading;

		dgvAttendance.Enabled =
			!loading;


		btnRefresh.Text =
			loading
				? "Loading..."
				: "Refresh";


		if (loading)
		{
			lblStatus.Text =
				"Loading event attendance...";

			lblStatus.Visible =
				true;
		}


		Cursor =
			loading
				? Cursors.WaitCursor
				: Cursors.Default;
	}


	private void ShowError(
		string message)
	{
		dgvAttendance.Rows.Clear();


		lblStatus.Text =
			message;

		lblStatus.Visible =
			true;


		lblAttendanceCount.Text =
			"0";
	}
	protected override void OnHandleDestroyed(
	EventArgs e)
	{
		StopFingerprintScanner();


		base.OnHandleDestroyed(
			e);
	}

	private void StopFingerprintScanner()
	{
		_scannerCancellationTokenSource?
			.Cancel();


		_scannerCancellationTokenSource?
			.Dispose();


		_scannerCancellationTokenSource =
			null;


		if (_fingerprintScanner is null)
		{
			return;
		}


		_fingerprintScanner.Connected -=
			FingerprintScanner_Connected;

		_fingerprintScanner.Disconnected -=
			FingerprintScanner_Disconnected;

		_fingerprintScanner.FingerprintCaptured -=
			FingerprintScanner_FingerprintCaptured;


		_fingerprintScanner.Stop();
	}

	private void lblScannerStatus_Click(object sender, EventArgs e)
	{

	}
}