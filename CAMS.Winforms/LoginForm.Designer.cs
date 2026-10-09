namespace CAMS.Winforms
{
	partial class LoginForm
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
			lblTitle = new Label();
			lbSubtitle = new Label();
			lblUsername = new Label();
			lblPassword = new Label();
			lblError = new Label();
			txtUserName = new MaskedTextBox();
			txtPassword = new MaskedTextBox();
			btnLogin = new Button();
			chkRememberMe = new CheckBox();
			SuspendLayout();
			// 
			// lblTitle
			// 
			lblTitle.AutoSize = true;
			lblTitle.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
			lblTitle.Location = new Point(106, 35);
			lblTitle.Name = "lblTitle";
			lblTitle.Size = new Size(68, 28);
			lblTitle.TabIndex = 0;
			lblTitle.Text = "CAMS";
			// 
			// lbSubtitle
			// 
			lbSubtitle.AutoSize = true;
			lbSubtitle.Location = new Point(88, 63);
			lbSubtitle.Name = "lbSubtitle";
			lbSubtitle.Size = new Size(109, 15);
			lbSubtitle.TabIndex = 1;
			lbSubtitle.Text = "Sta. Barbara Chapel";
			// 
			// lblUsername
			// 
			lblUsername.AutoSize = true;
			lblUsername.Location = new Point(51, 130);
			lblUsername.Name = "lblUsername";
			lblUsername.Size = new Size(60, 15);
			lblUsername.TabIndex = 2;
			lblUsername.Text = "Username";
			// 
			// lblPassword
			// 
			lblPassword.AutoSize = true;
			lblPassword.Location = new Point(51, 200);
			lblPassword.Name = "lblPassword";
			lblPassword.Size = new Size(57, 15);
			lblPassword.TabIndex = 3;
			lblPassword.Text = "Password";
			// 
			// lblError
			// 
			lblError.Font = new Font("Segoe UI", 8F);
			lblError.ForeColor = Color.Red;
			lblError.Location = new Point(51, 244);
			lblError.Name = "lblError";
			lblError.Size = new Size(179, 41);
			lblError.TabIndex = 4;
			lblError.Text = "Username";
			lblError.Visible = false;
			// 
			// txtUserName
			// 
			txtUserName.Location = new Point(51, 148);
			txtUserName.Name = "txtUserName";
			txtUserName.Size = new Size(179, 23);
			txtUserName.TabIndex = 5;
			// 
			// txtPassword
			// 
			txtPassword.Location = new Point(51, 218);
			txtPassword.Name = "txtPassword";
			txtPassword.PasswordChar = '*';
			txtPassword.Size = new Size(179, 23);
			txtPassword.TabIndex = 6;
			txtPassword.UseSystemPasswordChar = true;
			// 
			// btnLogin
			// 
			btnLogin.Location = new Point(51, 313);
			btnLogin.Name = "btnLogin";
			btnLogin.Size = new Size(179, 38);
			btnLogin.TabIndex = 7;
			btnLogin.Text = "Login";
			btnLogin.UseVisualStyleBackColor = true;
			btnLogin.Click += btnLogin_Click;
			// 
			// chkRememberMe
			// 
			chkRememberMe.AutoSize = true;
			chkRememberMe.Location = new Point(51, 288);
			chkRememberMe.Name = "chkRememberMe";
			chkRememberMe.Size = new Size(104, 19);
			chkRememberMe.TabIndex = 8;
			chkRememberMe.Text = "Remember Me";
			chkRememberMe.UseVisualStyleBackColor = true;
			// 
			// LoginForm
			// 
			AcceptButton = btnLogin;
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(280, 393);
			Controls.Add(chkRememberMe);
			Controls.Add(btnLogin);
			Controls.Add(txtPassword);
			Controls.Add(txtUserName);
			Controls.Add(lblError);
			Controls.Add(lblPassword);
			Controls.Add(lblUsername);
			Controls.Add(lbSubtitle);
			Controls.Add(lblTitle);
			Name = "LoginForm";
			Text = "LoginForm";
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private Label lblTitle;
		private Label lbSubtitle;
		private Label lblUsername;
		private Label lblPassword;
		private Label lblError;
		private MaskedTextBox txtUserName;
		private MaskedTextBox txtPassword;
		private Button btnLogin;
		private CheckBox chkRememberMe;
	}
}