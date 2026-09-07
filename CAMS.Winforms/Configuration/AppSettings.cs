using System.Text.Json;

namespace CAMS.Winforms.Configuration;

public sealed class AppSettings
{
	public string ApiBaseUrl { get; set; } =
		string.Empty;


	public static AppSettings Load()
	{
		var path =
			Path.Combine(
				AppContext.BaseDirectory,
				"appsettings.json");


		if (!File.Exists(path))
		{
			throw new FileNotFoundException(
				"appsettings.json was not found.",
				path);
		}


		var json =
			File.ReadAllText(
				path);


		var settings =
			JsonSerializer.Deserialize<AppSettings>(
				json,
				new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive =
						true
				});


		if (settings is null)
		{
			throw new InvalidOperationException(
				"Unable to load application settings.");
		}


		if (
			string.IsNullOrWhiteSpace(
				settings.ApiBaseUrl)
		)
		{
			throw new InvalidOperationException(
				"ApiBaseUrl is not configured.");
		}


		settings.ApiBaseUrl =
			settings.ApiBaseUrl
				.TrimEnd('/');


		return settings;
	}
}