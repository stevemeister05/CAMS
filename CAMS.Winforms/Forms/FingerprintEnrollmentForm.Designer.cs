namespace CAMS.Winforms.Forms
{
	partial class FingerprintEnrollmentForm
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
			lblMember = new Label();
			lblMemberValue = new Label();
			lblFinger = new Label();
			cmbFinger = new ComboBox();
			lblStatusCaption = new Label();
			lblStatus = new Label();
			lblQualityCaption = new Label();
			lblQuality = new Label();
			prgEnrollment = new ProgressBar();
			btnStart = new Button();
			btnCancel = new Button();
			SuspendLayout();
			// 
			// lblMember
			// 
			lblMember.AutoSize = true;
			lblMember.Location = new Point(38, 26);
			lblMember.Name = "lblMember";
			lblMember.Size = new Size(55, 15);
			lblMember.TabIndex = 0;
			lblMember.Text = "Member:";
			// 
			// lblMemberValue
			// 
			lblMemberValue.AutoSize = true;
			lblMemberValue.Location = new Point(99, 26);
			lblMemberValue.Name = "lblMemberValue";
			lblMemberValue.Size = new Size(38, 15);
			lblMemberValue.TabIndex = 1;
			lblMemberValue.Text = "label2";
			// 
			// lblFinger
			// 
			lblFinger.AutoSize = true;
			lblFinger.Location = new Point(38, 51);
			lblFinger.Name = "lblFinger";
			lblFinger.Size = new Size(43, 15);
			lblFinger.TabIndex = 2;
			lblFinger.Text = "Finger:";
			// 
			// cmbFinger
			// 
			cmbFinger.DropDownStyle = ComboBoxStyle.DropDownList;
			cmbFinger.FormattingEnabled = true;
			cmbFinger.Location = new Point(99, 43);
			cmbFinger.Name = "cmbFinger";
			cmbFinger.Size = new Size(121, 23);
			cmbFinger.TabIndex = 3;
			// 
			// lblStatusCaption
			// 
			lblStatusCaption.AutoSize = true;
			lblStatusCaption.Location = new Point(38, 76);
			lblStatusCaption.Name = "lblStatusCaption";
			lblStatusCaption.Size = new Size(42, 15);
			lblStatusCaption.TabIndex = 4;
			lblStatusCaption.Text = "Status:";
			// 
			// lblStatus
			// 
			lblStatus.AutoSize = true;
			lblStatus.Location = new Point(99, 76);
			lblStatus.Name = "lblStatus";
			lblStatus.Size = new Size(89, 15);
			lblStatus.TabIndex = 5;
			lblStatus.Text = "Ready to enroll.";
			// 
			// lblQualityCaption
			// 
			lblQualityCaption.AutoSize = true;
			lblQualityCaption.Location = new Point(38, 101);
			lblQualityCaption.Name = "lblQualityCaption";
			lblQualityCaption.Size = new Size(48, 15);
			lblQualityCaption.TabIndex = 6;
			lblQualityCaption.Text = "Quality:";
			// 
			// lblQuality
			// 
			lblQuality.AutoSize = true;
			lblQuality.Location = new Point(99, 101);
			lblQuality.Name = "lblQuality";
			lblQuality.Size = new Size(38, 15);
			lblQuality.TabIndex = 7;
			lblQuality.Text = "label7";
			// 
			// prgEnrollment
			// 
			prgEnrollment.Location = new Point(154, 256);
			prgEnrollment.Name = "prgEnrollment";
			prgEnrollment.Size = new Size(100, 23);
			prgEnrollment.Style = ProgressBarStyle.Marquee;
			prgEnrollment.TabIndex = 8;
			prgEnrollment.Visible = false;
			// 
			// btnStart
			// 
			btnStart.Location = new Point(38, 154);
			btnStart.Name = "btnStart";
			btnStart.Size = new Size(182, 33);
			btnStart.TabIndex = 9;
			btnStart.Text = "Start Enrollment";
			btnStart.UseVisualStyleBackColor = true;
			// 
			// btnCancel
			// 
			btnCancel.Location = new Point(38, 193);
			btnCancel.Name = "btnCancel";
			btnCancel.Size = new Size(182, 33);
			btnCancel.TabIndex = 10;
			btnCancel.Text = "Cancel";
			btnCancel.UseVisualStyleBackColor = true;
			// 
			// FingerprintEnrollmentForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(266, 291);
			Controls.Add(btnCancel);
			Controls.Add(btnStart);
			Controls.Add(prgEnrollment);
			Controls.Add(lblQuality);
			Controls.Add(lblQualityCaption);
			Controls.Add(lblStatus);
			Controls.Add(lblStatusCaption);
			Controls.Add(cmbFinger);
			Controls.Add(lblFinger);
			Controls.Add(lblMemberValue);
			Controls.Add(lblMember);
			Name = "FingerprintEnrollmentForm";
			Text = "FingerprintEnrollmentForm";
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private Label lblMember;
		private Label lblMemberValue;
		private Label lblFinger;
		private ComboBox cmbFinger;
		private Label lblStatusCaption;
		private Label lblStatus;
		private Label lblQualityCaption;
		private Label lblQuality;
		private ProgressBar prgEnrollment;
		private Button btnStart;
		private Button btnCancel;
	}
}