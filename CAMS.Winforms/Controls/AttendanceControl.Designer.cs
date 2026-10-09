namespace CAMS.Winforms.Controls
{
	partial class AttendanceControl
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
			lblEventDate = new Label();
			btnRefresh = new Button();
			lblEventName = new Label();
			btnBack = new Button();
			panel1 = new Panel();
			panel4 = new Panel();
			lblTimeOutWindow = new Label();
			lblTimeOutTitle = new Label();
			lblTimeInWindow = new Label();
			lblTimeInTitle = new Label();
			btnTimeOut = new Button();
			btnTimeIn = new Button();
			lblAttendanceMode = new Label();
			panel3 = new Panel();
			lblScannerMessage = new Label();
			lblScannerStatus = new Label();
			lblScannerTitle = new Label();
			panel2 = new Panel();
			dgvAttendance = new DataGridView();
			colMember = new DataGridViewTextBoxColumn();
			colTimeIn = new DataGridViewTextBoxColumn();
			colTimeOut = new DataGridViewTextBoxColumn();
			lblAttendanceCount = new Label();
			panel5 = new Panel();
			lblStatus = new Label();
			pnlHeader.SuspendLayout();
			panel1.SuspendLayout();
			panel4.SuspendLayout();
			panel3.SuspendLayout();
			panel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)dgvAttendance).BeginInit();
			panel5.SuspendLayout();
			SuspendLayout();
			// 
			// pnlHeader
			// 
			pnlHeader.Controls.Add(lblEventDate);
			pnlHeader.Controls.Add(btnRefresh);
			pnlHeader.Controls.Add(lblEventName);
			pnlHeader.Controls.Add(btnBack);
			pnlHeader.Dock = DockStyle.Top;
			pnlHeader.Location = new Point(0, 0);
			pnlHeader.Name = "pnlHeader";
			pnlHeader.Size = new Size(932, 65);
			pnlHeader.TabIndex = 2;
			// 
			// lblEventDate
			// 
			lblEventDate.Dock = DockStyle.Right;
			lblEventDate.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
			lblEventDate.Location = new Point(731, 0);
			lblEventDate.Name = "lblEventDate";
			lblEventDate.Size = new Size(132, 65);
			lblEventDate.TabIndex = 1;
			lblEventDate.Text = "Sta. Barbara Chapel";
			lblEventDate.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// btnRefresh
			// 
			btnRefresh.Dock = DockStyle.Right;
			btnRefresh.Location = new Point(863, 0);
			btnRefresh.Name = "btnRefresh";
			btnRefresh.Size = new Size(69, 65);
			btnRefresh.TabIndex = 2;
			btnRefresh.Text = "Refresh";
			btnRefresh.UseVisualStyleBackColor = true;
			btnRefresh.Click += btnRefresh_Click;
			// 
			// lblEventName
			// 
			lblEventName.Dock = DockStyle.Left;
			lblEventName.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
			lblEventName.Location = new Point(69, 0);
			lblEventName.Name = "lblEventName";
			lblEventName.Padding = new Padding(5, 0, 0, 0);
			lblEventName.Size = new Size(355, 65);
			lblEventName.TabIndex = 0;
			lblEventName.Text = "Today's Events";
			lblEventName.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// btnBack
			// 
			btnBack.Dock = DockStyle.Left;
			btnBack.Location = new Point(0, 0);
			btnBack.Name = "btnBack";
			btnBack.Size = new Size(69, 65);
			btnBack.TabIndex = 3;
			btnBack.Text = "Back";
			btnBack.UseVisualStyleBackColor = true;
			btnBack.Click += btnBack_Click;
			// 
			// panel1
			// 
			panel1.Controls.Add(panel4);
			panel1.Controls.Add(panel3);
			panel1.Dock = DockStyle.Left;
			panel1.Location = new Point(0, 65);
			panel1.Name = "panel1";
			panel1.Size = new Size(462, 479);
			panel1.TabIndex = 3;
			// 
			// panel4
			// 
			panel4.BorderStyle = BorderStyle.FixedSingle;
			panel4.Controls.Add(lblTimeOutWindow);
			panel4.Controls.Add(lblTimeOutTitle);
			panel4.Controls.Add(lblTimeInWindow);
			panel4.Controls.Add(lblTimeInTitle);
			panel4.Controls.Add(btnTimeOut);
			panel4.Controls.Add(btnTimeIn);
			panel4.Controls.Add(lblAttendanceMode);
			panel4.Dock = DockStyle.Fill;
			panel4.Location = new Point(0, 0);
			panel4.Name = "panel4";
			panel4.Size = new Size(462, 277);
			panel4.TabIndex = 1;
			// 
			// lblTimeOutWindow
			// 
			lblTimeOutWindow.AutoSize = true;
			lblTimeOutWindow.Location = new Point(69, 176);
			lblTimeOutWindow.Name = "lblTimeOutWindow";
			lblTimeOutWindow.Size = new Size(38, 15);
			lblTimeOutWindow.TabIndex = 6;
			lblTimeOutWindow.Text = "label3";
			// 
			// lblTimeOutTitle
			// 
			lblTimeOutTitle.AutoSize = true;
			lblTimeOutTitle.Location = new Point(69, 161);
			lblTimeOutTitle.Name = "lblTimeOutTitle";
			lblTimeOutTitle.Size = new Size(38, 15);
			lblTimeOutTitle.TabIndex = 5;
			lblTimeOutTitle.Text = "label2";
			// 
			// lblTimeInWindow
			// 
			lblTimeInWindow.AutoSize = true;
			lblTimeInWindow.Location = new Point(69, 122);
			lblTimeInWindow.Name = "lblTimeInWindow";
			lblTimeInWindow.Size = new Size(38, 15);
			lblTimeInWindow.TabIndex = 4;
			lblTimeInWindow.Text = "label1";
			// 
			// lblTimeInTitle
			// 
			lblTimeInTitle.AutoSize = true;
			lblTimeInTitle.Location = new Point(69, 107);
			lblTimeInTitle.Name = "lblTimeInTitle";
			lblTimeInTitle.Size = new Size(38, 15);
			lblTimeInTitle.TabIndex = 3;
			lblTimeInTitle.Text = "label1";
			// 
			// btnTimeOut
			// 
			btnTimeOut.Location = new Point(150, 43);
			btnTimeOut.Name = "btnTimeOut";
			btnTimeOut.Size = new Size(75, 23);
			btnTimeOut.TabIndex = 2;
			btnTimeOut.Text = "button1";
			btnTimeOut.UseVisualStyleBackColor = true;
			btnTimeOut.Click += btnTimeOut_Click;
			// 
			// btnTimeIn
			// 
			btnTimeIn.Location = new Point(69, 43);
			btnTimeIn.Name = "btnTimeIn";
			btnTimeIn.Size = new Size(75, 23);
			btnTimeIn.TabIndex = 1;
			btnTimeIn.Text = "button1";
			btnTimeIn.UseVisualStyleBackColor = true;
			btnTimeIn.Click += btnTimeIn_Click;
			// 
			// lblAttendanceMode
			// 
			lblAttendanceMode.AutoSize = true;
			lblAttendanceMode.Location = new Point(92, 25);
			lblAttendanceMode.Name = "lblAttendanceMode";
			lblAttendanceMode.Size = new Size(38, 15);
			lblAttendanceMode.TabIndex = 0;
			lblAttendanceMode.Text = "label1";
			// 
			// panel3
			// 
			panel3.BorderStyle = BorderStyle.Fixed3D;
			panel3.Controls.Add(lblScannerMessage);
			panel3.Controls.Add(lblScannerStatus);
			panel3.Controls.Add(lblScannerTitle);
			panel3.Dock = DockStyle.Bottom;
			panel3.Location = new Point(0, 277);
			panel3.Name = "panel3";
			panel3.Size = new Size(462, 202);
			panel3.TabIndex = 0;
			// 
			// lblScannerMessage
			// 
			lblScannerMessage.AutoSize = true;
			lblScannerMessage.Location = new Point(69, 93);
			lblScannerMessage.Name = "lblScannerMessage";
			lblScannerMessage.Size = new Size(38, 15);
			lblScannerMessage.TabIndex = 9;
			lblScannerMessage.Text = "label3";
			// 
			// lblScannerStatus
			// 
			lblScannerStatus.AutoSize = true;
			lblScannerStatus.Location = new Point(69, 78);
			lblScannerStatus.Name = "lblScannerStatus";
			lblScannerStatus.Size = new Size(38, 15);
			lblScannerStatus.TabIndex = 8;
			lblScannerStatus.Text = "label3";
			lblScannerStatus.Click += lblScannerStatus_Click;
			// 
			// lblScannerTitle
			// 
			lblScannerTitle.AutoSize = true;
			lblScannerTitle.Location = new Point(69, 19);
			lblScannerTitle.Name = "lblScannerTitle";
			lblScannerTitle.Size = new Size(38, 15);
			lblScannerTitle.TabIndex = 7;
			lblScannerTitle.Text = "label3";
			// 
			// panel2
			// 
			panel2.Controls.Add(dgvAttendance);
			panel2.Controls.Add(lblAttendanceCount);
			panel2.Dock = DockStyle.Fill;
			panel2.Location = new Point(462, 65);
			panel2.Name = "panel2";
			panel2.Size = new Size(470, 479);
			panel2.TabIndex = 4;
			// 
			// dgvAttendance
			// 
			dgvAttendance.AllowUserToAddRows = false;
			dgvAttendance.AllowUserToDeleteRows = false;
			dgvAttendance.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dgvAttendance.Columns.AddRange(new DataGridViewColumn[] { colMember, colTimeIn, colTimeOut });
			dgvAttendance.Dock = DockStyle.Fill;
			dgvAttendance.Location = new Point(0, 0);
			dgvAttendance.MultiSelect = false;
			dgvAttendance.Name = "dgvAttendance";
			dgvAttendance.ReadOnly = true;
			dgvAttendance.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			dgvAttendance.Size = new Size(470, 444);
			dgvAttendance.TabIndex = 3;
			// 
			// colMember
			// 
			colMember.HeaderText = "Member Name";
			colMember.Name = "colMember";
			colMember.ReadOnly = true;
			colMember.Width = 142;
			// 
			// colTimeIn
			// 
			colTimeIn.HeaderText = "Time In";
			colTimeIn.Name = "colTimeIn";
			colTimeIn.ReadOnly = true;
			colTimeIn.Width = 143;
			// 
			// colTimeOut
			// 
			colTimeOut.HeaderText = "Time Out";
			colTimeOut.Name = "colTimeOut";
			colTimeOut.ReadOnly = true;
			colTimeOut.Width = 142;
			// 
			// lblAttendanceCount
			// 
			lblAttendanceCount.Dock = DockStyle.Bottom;
			lblAttendanceCount.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
			lblAttendanceCount.Location = new Point(0, 444);
			lblAttendanceCount.Name = "lblAttendanceCount";
			lblAttendanceCount.Size = new Size(470, 35);
			lblAttendanceCount.TabIndex = 4;
			lblAttendanceCount.Text = "Sta. Barbara Chapel";
			lblAttendanceCount.TextAlign = ContentAlignment.MiddleRight;
			// 
			// panel5
			// 
			panel5.Controls.Add(lblStatus);
			panel5.Dock = DockStyle.Bottom;
			panel5.Location = new Point(0, 544);
			panel5.Name = "panel5";
			panel5.Size = new Size(932, 41);
			panel5.TabIndex = 5;
			// 
			// lblStatus
			// 
			lblStatus.Dock = DockStyle.Right;
			lblStatus.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
			lblStatus.Location = new Point(809, 0);
			lblStatus.Name = "lblStatus";
			lblStatus.Size = new Size(123, 41);
			lblStatus.TabIndex = 2;
			lblStatus.Text = "Sta. Barbara Chapel";
			lblStatus.TextAlign = ContentAlignment.MiddleLeft;
			lblStatus.Visible = false;
			// 
			// AttendanceControl
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			Controls.Add(panel2);
			Controls.Add(panel1);
			Controls.Add(pnlHeader);
			Controls.Add(panel5);
			Name = "AttendanceControl";
			Size = new Size(932, 585);
			pnlHeader.ResumeLayout(false);
			panel1.ResumeLayout(false);
			panel4.ResumeLayout(false);
			panel4.PerformLayout();
			panel3.ResumeLayout(false);
			panel3.PerformLayout();
			panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)dgvAttendance).EndInit();
			panel5.ResumeLayout(false);
			ResumeLayout(false);
		}

		#endregion

		private Panel pnlHeader;
		private Label lblEventDate;
		private Button btnRefresh;
		private Label lblEventName;
		private Button btnBack;
		private Panel panel1;
		private Panel panel2;
		private Panel panel4;
		private Panel panel3;
		private Button btnTimeIn;
		private Label lblAttendanceMode;
		private Button btnTimeOut;
		private Label lblTimeOutWindow;
		private Label lblTimeOutTitle;
		private Label lblTimeInWindow;
		private Label lblTimeInTitle;
		private Label lblScannerStatus;
		private Label lblScannerTitle;
		private DataGridView dgvAttendance;
		private DataGridViewTextBoxColumn colMember;
		private DataGridViewTextBoxColumn colTimeIn;
		private DataGridViewTextBoxColumn colTimeOut;
		private Panel panel5;
		private Label lblStatus;
		private Label lblAttendanceCount;
		private Label lblScannerMessage;
	}
}
