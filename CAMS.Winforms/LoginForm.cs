using CAMS.Application.Authentication;
using CAMS.Winforms.Services;

namespace CAMS.Winforms;

public partial class LoginForm
	: Form
{
	private readonly AuthenticationApiService?
		_authenticationService;


	public LoginForm()
	{
		InitializeComponent();

		InitializePage();
	}


	public LoginForm(
		AuthenticationApiService authenticationService)
		: this()
	{
		_authenticationService =
			authenticationService;
	}


	private void InitializePage()
	{
		Text =
			"CAMS - Sign In";

		StartPosition =
			FormStartPosition.CenterScreen;

		FormBorderStyle =
			FormBorderStyle.FixedSingle;

		MaximizeBox =
			false;


		AcceptButton =
			btnLogin;


		lblError.Visible =
			false;
	}


	private async void btnLogin_Click(
		object sender,
		EventArgs e)
	{
		await LoginAsync();
	}


	private async Task LoginAsync()
	{
		HideError();


		var userName =
			txtUserName.Text
				.Trim();

		var password =
			txtPassword.Text;


		if (
			string.IsNullOrWhiteSpace(
				userName)
		)
		{
			ShowError(
				"Username is required.");

			txtUserName.Focus();

			return;
		}


		if (
			string.IsNullOrWhiteSpace(
				password)
		)
		{
			ShowError(
				"Password is required.");

			txtPassword.Focus();

			return;
		}


		if (
			_authenticationService is null
		)
		{
			ShowError(
				"Authentication service is not available.");

			return;
		}


		SetLoading(
			true);


		try
		{
			var request =
				new LoginRequest
				{
					UserName =
						userName,

					Password =
						password,

					RememberMe =
						chkRememberMe.Checked
				};


			var result =
				await _authenticationService
					.LoginAsync(
						request);
			if (!result.Succeeded)
			{
				ShowError(
					result.Message ?? "Invalid username or password.");

				return;
			}

			if (
				result.MustChangePassword
			)
			{
				ShowError(
					"You must change your password before using CAMS WinForms.");

				return;
			}


			if (
				!CanAccessWinForms(
					result.Roles)
			)
			{
				ShowError(
					"You do not have permission to use CAMS WinForms.");

				return;
			}


			DialogResult = DialogResult.OK;
			Close();
		}
		catch (HttpRequestException)
		{
			ShowError(
				"Unable to connect to the CAMS server.");
		}
		catch (TaskCanceledException)
		{
			ShowError(
				"The connection to the CAMS server timed out.");
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


	private static bool CanAccessWinForms(
		IEnumerable<string> roles)
	{
		return roles.Any(
			role =>
				role.Equals(
					"Administrator",
					StringComparison.OrdinalIgnoreCase) ||
				role.Equals(
					"AttendanceStaff",
					StringComparison.OrdinalIgnoreCase));
	}


	private void SetLoading(
		bool loading)
	{
		txtUserName.Enabled =
			!loading;

		txtPassword.Enabled =
			!loading;

		chkRememberMe.Enabled =
			!loading;

		btnLogin.Enabled =
			!loading;


		btnLogin.Text =
			loading
				? "Signing in..."
				: "Sign In";


		Cursor =
			loading
				? Cursors.WaitCursor
				: Cursors.Default;
	}


	private void ShowError(
		string message)
	{
		lblError.Text =
			message;

		lblError.Visible =
			true;
	}


	private void HideError()
	{
		lblError.Text =
			string.Empty;

		lblError.Visible =
			false;
	}
}