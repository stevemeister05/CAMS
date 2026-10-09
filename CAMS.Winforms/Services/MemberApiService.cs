using CAMS.Application.Common;
using CAMS.Application.Member.DTOs;
using CAMS.Winforms.Api;

namespace CAMS.Winforms.Services;

public sealed class MemberApiService
{
	private readonly CamsApiClient
		_apiClient;


	public MemberApiService(
		CamsApiClient apiClient)
	{
		_apiClient =
			apiClient;
	}


	public async Task<MemberFingerprintResponse>
		EnrollFingerprintAsync(
			Guid memberId,
			byte[] template,
			string? fingerLabel,
			CancellationToken cancellationToken = default)
	{
		if (memberId == Guid.Empty)
		{
			throw new ArgumentException(
				"Member ID is required.",
				nameof(memberId));
		}


		ArgumentNullException.ThrowIfNull(
			template);


		if (template.Length == 0)
		{
			throw new ArgumentException(
				"Fingerprint template is required.",
				nameof(template));
		}


		var request =
			new EnrollFingerprintRequest
			{
				Template =
					template,

				FingerLabel =
					string.IsNullOrWhiteSpace(
						fingerLabel)
						? null
						: fingerLabel.Trim()
			};


		using var response =
			await _apiClient
				.PostAsync(
					$"/api/v1/member/{memberId}/fingerprint",
					request,
					cancellationToken);


		if (!response.IsSuccessStatusCode)
		{
			var message =
				await _apiClient
					.GetErrorMessageAsync(
						response,
						"Unable to enroll fingerprint.",
						cancellationToken);


			throw new InvalidOperationException(
				message);
		}


		var result =
			await _apiClient
				.ReadAsync<ApiResult<MemberFingerprintResponse>>(
						response,
						cancellationToken);


		if (
			result is null ||
			result.Data is null
		)
		{
			throw new InvalidOperationException(
				"The CAMS server returned an invalid " +
				"fingerprint enrollment response.");
		}


		return result.Data;
	}
}