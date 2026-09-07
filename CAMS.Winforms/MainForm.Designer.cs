namespace CAMS.Winforms
{
	partial class MainForm
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			pnlHeader = new Panel();
			lblPageTitle = new Label();
			lblAppName = new Label();
			pnlSidebar = new Panel();
			btnLogout = new Button();
			btnSettings = new Button();
			btnScanner = new Button();
			btnFingerprintRegistration = new Button();
			btnTodaysEvents = new Button();
			pnlContent = new Panel();
			lblContentPlaceholder = new Label();
			pnlHeader.SuspendLayout();
			pnlSidebar.SuspendLayout();
			pnlContent.SuspendLayout();
			SuspendLayout();
			// 
			// pnlHeader
			// 
			pnlHeader.Controls.Add(lblPageTitle);
			pnlHeader.Controls.Add(lblAppName);
			pnlHeader.Dock = DockStyle.Top;
			pnlHeader.Location = new Point(0, 0);
			pnlHeader.Name = "pnlHeader";
			pnlHeader.Size = new Size(1009, 65);
			pnlHeader.TabIndex = 0;
			// 
			// lblPageTitle
			// 
			lblPageTitle.Dock = DockStyle.Right;
			lblPageTitle.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
			lblPageTitle.Location = new Point(781, 0);
			lblPageTitle.Name = "lblPageTitle";
			lblPageTitle.Size = new Size(228, 65);
			lblPageTitle.TabIndex = 1;
			lblPageTitle.Text = "Sta. Barbara Chapel";
			lblPageTitle.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// lblAppName
			// 
			lblAppName.Dock = DockStyle.Left;
			lblAppName.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
			lblAppName.Location = new Point(0, 0);
			lblAppName.Name = "lblAppName";
			lblAppName.Padding = new Padding(5, 0, 0, 0);
			lblAppName.Size = new Size(524, 65);
			lblAppName.TabIndex = 0;
			lblAppName.Text = "Church Attendance Management System";
			lblAppName.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// pnlSidebar
			// 
			pnlSidebar.Controls.Add(btnLogout);
			pnlSidebar.Controls.Add(btnSettings);
			pnlSidebar.Controls.Add(btnScanner);
			pnlSidebar.Controls.Add(btnFingerprintRegistration);
			pnlSidebar.Controls.Add(btnTodaysEvents);
			pnlSidebar.Dock = DockStyle.Left;
			pnlSidebar.Location = new Point(0, 65);
			pnlSidebar.Name = "pnlSidebar";
			pnlSidebar.Size = new Size(230, 531);
			pnlSidebar.TabIndex = 1;
			// 
			// btnLogout
			// 
			btnLogout.Dock = DockStyle.Top;
			btnLogout.FlatAppearance.BorderSize = 0;
			btnLogout.FlatStyle = FlatStyle.Flat;
			btnLogout.Location = new Point(0, 192);
			btnLogout.Name = "btnLogout";
			btnLogout.Size = new Size(230, 48);
			btnLogout.TabIndex = 4;
			btnLogout.Text = "Logout";
			btnLogout.TextAlign = ContentAlignment.MiddleLeft;
			btnLogout.UseVisualStyleBackColor = true;
			// 
			// btnSettings
			// 
			btnSettings.Dock = DockStyle.Top;
			btnSettings.FlatAppearance.BorderSize = 0;
			btnSettings.FlatStyle = FlatStyle.Flat;
			btnSettings.Location = new Point(0, 144);
			btnSettings.Name = "btnSettings";
			btnSettings.Size = new Size(230, 48);
			btnSettings.TabIndex = 3;
			btnSettings.Text = "Settings";
			btnSettings.TextAlign = ContentAlignment.MiddleLeft;
			btnSettings.UseVisualStyleBackColor = true;
			// 
			// btnScanner
			// 
			btnScanner.Dock = DockStyle.Top;
			btnScanner.FlatAppearance.BorderSize = 0;
			btnScanner.FlatStyle = FlatStyle.Flat;
			btnScanner.Location = new Point(0, 96);
			btnScanner.Name = "btnScanner";
			btnScanner.Size = new Size(230, 48);
			btnScanner.TabIndex = 2;
			btnScanner.Text = "Scanner";
			btnScanner.TextAlign = ContentAlignment.MiddleLeft;
			btnScanner.UseVisualStyleBackColor = true;
			// 
			// btnFingerprintRegistration
			// 
			btnFingerprintRegistration.Dock = DockStyle.Top;
			btnFingerprintRegistration.FlatAppearance.BorderSize = 0;
			btnFingerprintRegistration.FlatStyle = FlatStyle.Flat;
			btnFingerprintRegistration.Location = new Point(0, 48);
			btnFingerprintRegistration.Name = "btnFingerprintRegistration";
			btnFingerprintRegistration.Size = new Size(230, 48);
			btnFingerprintRegistration.TabIndex = 1;
			btnFingerprintRegistration.Text = "Fingerprint Registration";
			btnFingerprintRegistration.TextAlign = ContentAlignment.MiddleLeft;
			btnFingerprintRegistration.UseVisualStyleBackColor = true;
			// 
			// btnTodaysEvents
			// 
			btnTodaysEvents.Dock = DockStyle.Top;
			btnTodaysEvents.FlatAppearance.BorderSize = 0;
			btnTodaysEvents.FlatStyle = FlatStyle.Flat;
			btnTodaysEvents.Location = new Point(0, 0);
			btnTodaysEvents.Name = "btnTodaysEvents";
			btnTodaysEvents.Size = new Size(230, 48);
			btnTodaysEvents.TabIndex = 0;
			btnTodaysEvents.Text = "Today's Events";
			btnTodaysEvents.TextAlign = ContentAlignment.MiddleLeft;
			btnTodaysEvents.UseVisualStyleBackColor = true;
			btnTodaysEvents.Click += btnTodayEvents_Click;
			// 
			// pnlContent
			// 
			pnlContent.Controls.Add(lblContentPlaceholder);
			pnlContent.Dock = DockStyle.Fill;
			pnlContent.Location = new Point(230, 65);
			pnlContent.Name = "pnlContent";
			pnlContent.Size = new Size(779, 531);
			pnlContent.TabIndex = 2;
			// 
			// lblContentPlaceholder
			// 
			lblContentPlaceholder.Dock = DockStyle.Top;
			lblContentPlaceholder.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
			lblContentPlaceholder.Location = new Point(0, 0);
			lblContentPlaceholder.Name = "lblContentPlaceholder";
			lblContentPlaceholder.Padding = new Padding(5, 0, 0, 0);
			lblContentPlaceholder.Size = new Size(779, 45);
			lblContentPlaceholder.TabIndex = 1;
			lblContentPlaceholder.Text = "label1";
			lblContentPlaceholder.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// MainForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(1009, 596);
			Controls.Add(pnlContent);
			Controls.Add(pnlSidebar);
			Controls.Add(pnlHeader);
			Name = "MainForm";
			Text = "MainForm";
			pnlHeader.ResumeLayout(false);
			pnlSidebar.ResumeLayout(false);
			pnlContent.ResumeLayout(false);
			ResumeLayout(false);
		}

		#endregion

		private Panel pnlHeader;
		private Panel pnlSidebar;
		private Panel pnlContent;
		private Label lblAppName;
		private Button btnTodaysEvents;
		private Button btnLogout;
		private Button btnSettings;
		private Button btnScanner;
		private Button btnFingerprintRegistration;
		private Label lblPageTitle;
		private Label lblContentPlaceholder;
	}
}