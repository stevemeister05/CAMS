using CAMS.Application.Event;
using CAMS.Application.Event.DTOs;
using CAMS.Winforms.Api;

namespace CAMS.Winforms.Services;

public sealed class EventApiService
{
	private readonly CamsApiClient
		_apiClient;


	public EventApiService(
		CamsApiClient apiClient)
	{
		_apiClient =
			apiClient;
	}


	public async Task<IReadOnlyList<EventResponse>>
		GetEventsAsync(
			CancellationToken cancellationToken = default)
	{
		using var response =
			await _apiClient.GetAsync(
				"/api/v1/events",
				cancellationToken);


		if (!response.IsSuccessStatusCode)
		{
			var message =
				await _apiClient.GetErrorMessageAsync(
					response,
					"Unable to load events.",
					cancellationToken);


			throw new InvalidOperationException(
				message);
		}


		var result =
			await _apiClient.ReadAsync<EventListResponse>(
				response,
				cancellationToken);


		return result?.Items ??
			[];
	}


	private sealed class EventListResponse
	{
		public List<EventResponse> Items { get; set; } =
			[];
	}
}