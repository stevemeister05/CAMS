namespace CAMS.Winforms.Controls
{
	partial class TodayEventsControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			pnlHeader = new Panel();
			lblDate = new Label();
			btnRefresh = new Button();
			lblTitle = new Label();
			dgvEvents = new DataGridView();
			panel1 = new Panel();
			lblStatus = new Label();
			Event = new DataGridViewTextBoxColumn();
			Schedule = new DataGridViewTextBoxColumn();
			Type = new DataGridViewTextBoxColumn();
			Status = new DataGridViewTextBoxColumn();
			colAttendance = new DataGridViewButtonColumn();
			pnlHeader.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)dgvEvents).BeginInit();
			panel1.SuspendLayout();
			SuspendLayout();
			// 
			// pnlHeader
			// 
			pnlHeader.Controls.Add(lblDate);
			pnlHeader.Controls.Add(btnRefresh);
			pnlHeader.Controls.Add(lblTitle);
			pnlHeader.Dock = DockStyle.Top;
			pnlHeader.Location = new Point(0, 0);
			pnlHeader.Name = "pnlHeader";
			pnlHeader.Size = new Size(945, 65);
			pnlHeader.TabIndex = 1;
			// 
			// lblDate
			// 
			lblDate.Dock = DockStyle.Right;
			lblDate.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
			lblDate.Location = new Point(744, 0);
			lblDate.Name = "lblDate";
			lblDate.Size = new Size(132, 65);
			lblDate.TabIndex = 1;
			lblDate.Text = "Sta. Barbara Chapel";
			lblDate.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// btnRefresh
			// 
			btnRefresh.Dock = DockStyle.Right;
			btnRefresh.Location = new Point(876, 0);
			btnRefresh.Name = "btnRefresh";
			btnRefresh.Size = new Size(69, 65);
			btnRefresh.TabIndex = 2;
			btnRefresh.Text = "Refresh";
			btnRefresh.UseVisualStyleBackColor = true;
			// 
			// lblTitle
			// 
			lblTitle.Dock = DockStyle.Left;
			lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
			lblTitle.Location = new Point(0, 0);
			lblTitle.Name = "lblTitle";
			lblTitle.Padding = new Padding(5, 0, 0, 0);
			lblTitle.Size = new Size(355, 65);
			lblTitle.TabIndex = 0;
			lblTitle.Text = "Today's Events";
			lblTitle.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// dgvEvents
			// 
			dgvEvents.AllowUserToAddRows = false;
			dgvEvents.AllowUserToDeleteRows = false;
			dgvEvents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			dgvEvents.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dgvEvents.Columns.AddRange(new DataGridViewColumn[] { Event, Schedule, Type, Status, colAttendance });
			dgvEvents.Dock = DockStyle.Fill;
			dgvEvents.Location = new Point(0, 65);
			dgvEvents.MultiSelect = false;
			dgvEvents.Name = "dgvEvents";
			dgvEvents.ReadOnly = true;
			dgvEvents.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			dgvEvents.Size = new Size(945, 436);
			dgvEvents.TabIndex = 2;
			// 
			// panel1
			// 
			panel1.Controls.Add(lblStatus);
			panel1.Dock = DockStyle.Bottom;
			panel1.Location = new Point(0, 501);
			panel1.Name = "panel1";
			panel1.Size = new Size(945, 41);
			panel1.TabIndex = 3;
			// 
			// lblStatus
			// 
			lblStatus.Dock = DockStyle.Right;
			lblStatus.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
			lblStatus.Location = new Point(822, 0);
			lblStatus.Name = "lblStatus";
			lblStatus.Size = new Size(123, 41);
			lblStatus.TabIndex = 2;
			lblStatus.Text = "Sta. Barbara Chapel";
			lblStatus.TextAlign = ContentAlignment.MiddleLeft;
			lblStatus.Visible = false;
			// 
			// Event
			// 
			Event.HeaderText = "Event";
			Event.Name = "Event";
			Event.ReadOnly = true;
			// 
			// Schedule
			// 
			Schedule.HeaderText = "Schedule";
			Schedule.Name = "Schedule";
			Schedule.ReadOnly = true;
			// 
			// Type
			// 
			Type.HeaderText = "Type";
			Type.Name = "Type";
			Type.ReadOnly = true;
			// 
			// Status
			// 
			Status.HeaderText = "Status";
			Status.Name = "Status";
			Status.ReadOnly = true;
			// 
			// colAttendance
			// 
			colAttendance.HeaderText = "Action";
			colAttendance.Name = "colAttendance";
			colAttendance.ReadOnly = true;
			colAttendance.Resizable = DataGridViewTriState.True;
			colAttendance.SortMode = DataGridViewColumnSortMode.Automatic;
			// 
			// TodayEventsControl
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			Controls.Add(dgvEvents);
			Controls.Add(panel1);
			Controls.Add(pnlHeader);
			Name = "TodayEventsControl";
			Size = new Size(945, 542);
			pnlHeader.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)dgvEvents).EndInit();
			panel1.ResumeLayout(false);
			ResumeLayout(false);
		}

		#endregion

		private Panel pnlHeader;
		private Label lblDate;
		private Label lblTitle;
		private Button btnRefresh;
		private DataGridView dgvEvents;
		private Panel panel1;
		private Label lblStatus;
		private DataGridViewTextBoxColumn Event;
		private DataGridViewTextBoxColumn Schedule;
		private DataGridViewTextBoxColumn Type;
		private DataGridViewTextBoxColumn Status;
		private DataGridViewButtonColumn colAttendance;
	}
}
