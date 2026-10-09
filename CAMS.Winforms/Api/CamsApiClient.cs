using CAMS.Winforms.Configuration;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace CAMS.Winforms.Api;

public sealed class CamsApiClient
	: IDisposable
{
	private readonly CookieContainer
		_cookieContainer;

	private readonly HttpClientHandler
		_handler;

	private readonly HttpClient
		_httpClient;

	private readonly JsonSerializerOptions
		_jsonOptions;


	public CamsApiClient(
		AppSettings settings)
	{
		_cookieContainer =
			new CookieContainer();


		_handler =
			new HttpClientHandler
			{
				CookieContainer =
					_cookieContainer,

				UseCookies =
					true
			};


		_httpClient =
			new HttpClient(
				_handler)
			{
				BaseAddress =
					new Uri(
						settings.ApiBaseUrl),

				Timeout =
					TimeSpan.FromSeconds(
						30)
			};


		_httpClient
			.DefaultRequestHeaders
			.Accept
			.Add(
				new MediaTypeWithQualityHeaderValue(
					"application/json"));


		_jsonOptions =
			new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive =
					true
			};
	}


	public async Task<HttpResponseMessage>
		GetAsync(
			string endpoint,
			CancellationToken cancellationToken = default)
	{
		return await _httpClient
			.GetAsync(
				endpoint,
				cancellationToken);
	}


	public async Task<HttpResponseMessage>
		PostAsync<T>(
			string endpoint,
			T request,
			CancellationToken cancellationToken = default)
	{
		var json =
			JsonSerializer.Serialize(
				request,
				_jsonOptions);


		using var content =
			new StringContent(
				json,
				Encoding.UTF8,
				"application/json");


		return await _httpClient
			.PostAsync(
				endpoint,
				content,
				cancellationToken);
	}


	public async Task<T?>
		ReadAsync<T>(
			HttpResponseMessage response,
			CancellationToken cancellationToken = default)
	{
		var content =
			await response.Content
				.ReadAsStringAsync(
					cancellationToken);


		if (
			string.IsNullOrWhiteSpace(
				content)
		)
		{
			return default;
		}


		return JsonSerializer.Deserialize<T>(
			content,
			_jsonOptions);
	}


	public async Task<string>
		GetErrorMessageAsync(
			HttpResponseMessage response,
			string fallback,
			CancellationToken cancellationToken = default)
	{
		var content =
			await response.Content
				.ReadAsStringAsync(
					cancellationToken);


		if (
			string.IsNullOrWhiteSpace(
				content)
		)
		{
			return fallback;
		}


		try
		{
			using var document =
				JsonDocument.Parse(
					content);


			var root =
				document.RootElement;


			if (
				root.TryGetProperty(
					"message",
					out var message) &&
				message.ValueKind ==
					JsonValueKind.String
			)
			{
				return message.GetString() ??
					fallback;
			}


			if (
				root.TryGetProperty(
					"title",
					out var title) &&
				title.ValueKind ==
					JsonValueKind.String
			)
			{
				return title.GetString() ??
					fallback;
			}
		}
		catch (JsonException)
		{
			// Return fallback below.
		}


		return fallback;
	}


	public void Dispose()
	{
		_httpClient.Dispose();

		_handler.Dispose();
	}
}