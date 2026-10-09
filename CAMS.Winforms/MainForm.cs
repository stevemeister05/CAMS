using CAMS.Winforms.Api;
using CAMS.Winforms.Controls;
using CAMS.Winforms.Fingerprint;
using CAMS.Winforms.Forms;
using CAMS.Winforms.Services;

namespace CAMS.Winforms;

public partial class MainForm
	: Form
{
	private CamsApiClient?
		_apiClient;

	private EventApiService?
		_eventService;

	private AttendanceApiService?
		_attendanceService;

	private MemberApiService?
		_memberService;


	private IFingerprintEnroller?
		_fingerprintEnroller;

	private IFingerprintScanner?
		_fingerprintScanner;

	public MainForm()
	{
		InitializeComponent();

		InitializePage();
	}


	public MainForm(
		CamsApiClient apiClient) : this()
	{
		_apiClient =
			apiClient;


		_eventService =
			new EventApiService(
				apiClient);


		_attendanceService =
			new AttendanceApiService(
				apiClient);

		_memberService =
			new MemberApiService(
				apiClient);

		_fingerprintEnroller =
			new FutronicFingerprintEnroller();

		ShowTodayEvents();
	}


	private void InitializePage()
	{
		Text =
			"CAMS - Church Attendance Management System";

		StartPosition =
			FormStartPosition.CenterScreen;

		WindowState =
			FormWindowState.Maximized;

		MinimumSize =
			new Size(
				1100,
				700);
	}


	private void ShowTodayEvents()
	{
		if (_eventService is null)
		{
			return;
		}


		lblPageTitle.Text =
			"Today's Events";


		var control =
			new TodayEventsControl(
				_eventService);


		control.AttendanceRequested +=
			TodayEventsControl_AttendanceRequested;


		ShowContent(
			control);
	}

	private void TodayEventsControl_AttendanceRequested(
		object? sender,
		Guid eventId)
	{
		ShowEventAttendance(
			eventId);
	}

	private void ShowEventAttendance(Guid eventId)
	{
		if (
			_attendanceService is null ||
			_fingerprintScanner is null
		)
		{
			return;
		}


		lblPageTitle.Text =
			"Event Attendance";


		var control =
			new AttendanceControl(
				eventId,
				_attendanceService,
				_fingerprintScanner);


		control.BackRequested +=
			AttendanceControl_BackRequested;


		ShowContent(
			control);
	}


	private void AttendanceControl_BackRequested(
		object? sender,
		EventArgs e)
	{
		ShowTodayEvents();
	}


	private void ShowContent(
		Control control)
	{
		pnlContent.SuspendLayout();


		try
		{
			foreach (
				Control existingControl in
				pnlContent.Controls
					.Cast<Control>()
					.ToList()
			)
			{
				existingControl.Dispose();
			}


			pnlContent.Controls.Clear();


			control.Dock =
				DockStyle.Fill;


			pnlContent.Controls.Add(
				control);
		}
		finally
		{
			pnlContent.ResumeLayout();
		}
	}


	private void btnTodayEvents_Click(
		object sender,
		EventArgs e)
	{
		ShowTodayEvents();
	}

	private DialogResult ShowFingerprintEnrollment(
		Guid memberId,
		string memberName)
	{
		if (
			_memberService is null ||
			_fingerprintEnroller is null
		)
		{
			MessageBox.Show(
				this,
				"Fingerprint enrollment is not available.",
				"CAMS",
				MessageBoxButtons.OK,
				MessageBoxIcon.Warning);


			return
				DialogResult.Abort;
		}


		using var form =
			new FingerprintEnrollmentForm(
				memberId,
				memberName,
				_memberService,
				_fingerprintEnroller);


		return
			form.ShowDialog(
				this);
	}
}