using CAMS.Application.Common.Filtering;
using CAMS.Domain.Entities;
using CAMS.Domain.Enums;

namespace CAMS.Application.Registration.DTOs;

public class RegistrationFilter : IFilter<RegistrationRequest>
{
	public RegistrationStatus? Status { get; set; }

	public string? MobileNumber { get; set; }

	public string? Search { get; set; }

	public IQueryable<RegistrationRequest> Filter(
		IQueryable<RegistrationRequest> query)
	{
		if (Status.HasValue)
		{
			query = query.Where(
				x => x.Status == Status.Value);
		}

		if (!string.IsNullOrWhiteSpace(MobileNumber))
		{
			var mobileNumber = MobileNumber.Trim();

			query = query.Where(
				x => x.MobileNumber == mobileNumber);
		}

		if (!string.IsNullOrWhiteSpace(Search))
		{
			var search = Search.Trim();

			query = query.Where(
				x =>
					x.FirstName.Contains(search) ||
					(x.MiddleName != null &&
					 x.MiddleName.Contains(search)) ||
					x.LastName.Contains(search) ||
					x.MobileNumber.Contains(search));
		}

		return query;
	}
}
