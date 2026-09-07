using CAMS.Application.Attendance.DTOs;
using CAMS.Winforms.Api;

namespace CAMS.Winforms.Services;

public sealed class AttendanceApiService
{
	private readonly CamsApiClient
		_apiClient;


	public AttendanceApiService(
		CamsApiClient apiClient)
	{
		_apiClient =
			apiClient;
	}


	public async Task<AttendancePageResponse>
		GetByEventAsync(
			Guid eventId,
			CancellationToken cancellationToken = default)
	{
		using var response =
			await _apiClient.GetAsync(
				$"/api/v1/attendance/event/{eventId}",
				cancellationToken);


		if (!response.IsSuccessStatusCode)
		{
			var message =
				await _apiClient.GetErrorMessageAsync(
					response,
					"Unable to load event attendance.",
					cancellationToken);


			throw new InvalidOperationException(
				message);
		}


		var result =
			await _apiClient.ReadAsync<AttendancePageResponse>(
				response,
				cancellationToken);


		if (result is null)
		{
			throw new InvalidOperationException(
				"Invalid attendance response received from the CAMS server.");
		}


		return result;
	}
}