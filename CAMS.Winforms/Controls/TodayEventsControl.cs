using CAMS.Application.Event;
using CAMS.Application.Event.DTOs;
using CAMS.Winforms.Services;

namespace CAMS.Winforms.Controls;

public partial class TodayEventsControl
	: UserControl
{
	private readonly EventApiService?
		_eventService;

	private IReadOnlyList<EventResponse>
		_events =
			[];


	public event EventHandler<Guid>?
		AttendanceRequested;


	public TodayEventsControl()
	{
		InitializeComponent();

		InitializePage();
	}


	public TodayEventsControl(
		EventApiService eventService)
		: this()
	{
		_eventService =
			eventService;
	}


	private void InitializePage()
	{
		Dock =
			DockStyle.Fill;


		lblDate.Text =
			DateTime.Now.ToString(
				"dddd, MMMM d, yyyy");


		ConfigureGrid();
	}


	private void ConfigureGrid()
	{
		dgvEvents.AutoGenerateColumns =
			false;

		dgvEvents.ReadOnly =
			true;

		dgvEvents.AllowUserToAddRows =
			false;

		dgvEvents.AllowUserToDeleteRows =
			false;

		dgvEvents.AllowUserToResizeRows =
			false;

		dgvEvents.MultiSelect =
			false;

		dgvEvents.SelectionMode =
			DataGridViewSelectionMode.FullRowSelect;

		dgvEvents.RowHeadersVisible =
			false;


		// Attendance button column
		colAttendance.Visible =
			true;

		colAttendance.HeaderText =
			"Action";

		colAttendance.Text =
			"Record Attendance";

		colAttendance.UseColumnTextForButtonValue =
			true;

		colAttendance.Width =
			160;

		colAttendance.AutoSizeMode =
			DataGridViewAutoSizeColumnMode.None;

		colAttendance.DisplayIndex =
			dgvEvents.Columns.Count - 1;


		dgvEvents.CellContentClick -=
			dgvEvents_CellContentClick;

		dgvEvents.CellContentClick +=
			dgvEvents_CellContentClick;
	}


	protected override async void OnLoad(
		EventArgs e)
	{
		base.OnLoad(
			e);


		if (
			DesignMode ||
			_eventService is null
		)
		{
			return;
		}


		await LoadEventsAsync();
	}


	private async void btnRefresh_Click(
		object sender,
		EventArgs e)
	{
		await LoadEventsAsync();
	}


	private async Task LoadEventsAsync()
	{
		if (_eventService is null)
		{
			ShowError(
				"Event service is not available.");

			return;
		}


		SetLoading(
			true);


		try
		{
			var events =
				await _eventService
					.GetEventsAsync();


			var today =
				DateOnly.FromDateTime(
					DateTime.Now);


			_events =
				events
					.Where(
						@event =>
							@event.EventDate ==
							today)
					.OrderBy(
						@event =>
							@event.StartTime)
					.ToList();


			RenderEvents();
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


	private void RenderEvents()
	{
		dgvEvents.Rows.Clear();


		if (_events.Count == 0)
		{
			lblStatus.Text =
				"There are no events scheduled for today.";

			lblStatus.Visible =
				true;

			return;
		}


		lblStatus.Visible =
			false;


		foreach (
			var @event in
			_events
		)
		{
			var rowIndex =
				dgvEvents.Rows.Add(
					@event.Name,
					FormatSchedule(
						@event.StartTime,
						@event.EndTime),
					@event.Type.ToString(),
					@event.Status.ToString());


			var row =
				dgvEvents.Rows[
					rowIndex
				];


			row.Tag =
				@event;
		}
	}


	private void dgvEvents_CellContentClick(
		object? sender,
		DataGridViewCellEventArgs e)
	{
		if (
			e.RowIndex < 0 ||
			e.ColumnIndex < 0
		)
		{
			return;
		}


		if (
			e.ColumnIndex !=
			colAttendance.Index
		)
		{
			return;
		}


		var @event =
			dgvEvents.Rows[
				e.RowIndex
			].Tag as EventResponse;


		if (@event is null)
		{
			return;
		}


		AttendanceRequested?.Invoke(
			this,
			@event.Id);
	}


	private static string FormatSchedule(
		TimeOnly startTime,
		TimeOnly endTime)
	{
		return
			startTime.ToString(
				"h:mm tt") +
			" - " +
			endTime.ToString(
				"h:mm tt");
	}


	private void SetLoading(
		bool loading)
	{
		btnRefresh.Enabled =
			!loading;

		dgvEvents.Enabled =
			!loading;


		btnRefresh.Text =
			loading
				? "Loading..."
				: "Refresh";


		if (loading)
		{
			lblStatus.Text =
				"Loading today's events...";

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
		dgvEvents.Rows.Clear();


		lblStatus.Text =
			message;

		lblStatus.Visible =
			true;
	}
}