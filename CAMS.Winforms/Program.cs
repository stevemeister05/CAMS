using CAMS.Winforms.Api;
using CAMS.Winforms.Configuration;
using CAMS.Winforms.Services;

namespace CAMS.Winforms;

internal static class Program
{
	[STAThread]
	static void Main()
	{
		ApplicationConfiguration.Initialize();


		try
		{
			var settings =
				AppSettings.Load();


			using var apiClient =
				new CamsApiClient(
					settings);


			var authenticationService =
				new AuthenticationApiService(
					apiClient);


			using var loginForm =
				new LoginForm(
					authenticationService);


			var loginResult =
				loginForm.ShowDialog();


			if (
				loginResult !=
				DialogResult.OK
			)
			{
				return;
			}


			System.Windows.Forms.Application.Run(
				new MainForm(
					apiClient));
		}
		catch (Exception exception)
		{
			MessageBox.Show(
				exception.Message,
				"CAMS",
				MessageBoxButtons.OK,
				MessageBoxIcon.Error);
		}
	}
}