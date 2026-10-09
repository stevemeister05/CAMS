using CAMS.Application.Common.Filtering;
using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Application.Member.DTOs;

public sealed class MemberFilter : IFilter<CAMS.Domain.Entities.Member>
{
	public string? Search { get; set; }

	public bool? IsActive { get; set; }

	public string? Gender { get; set; }

	public int? AttendanceCountFrom { get; set; }

	public int? AttendanceCountTo { get; set; }

	public IQueryable<CAMS.Domain.Entities.Member> Filter(IQueryable<CAMS.Domain.Entities.Member> query)
	{
		if (!string.IsNullOrWhiteSpace(Search))
		{
			var search = Search.Trim();

			query = query.Where(x =>
				x.FirstName.Contains(search) ||
				x.LastName.Contains(search) ||
				x.MobileNumber.Contains(search));
		}

		if (IsActive.HasValue)
		{
			query = query.Where(x =>
				x.IsActive == IsActive.Value);
		}

		if (!string.IsNullOrWhiteSpace(Gender))
		{
			query = query.Where(x =>
				x.Gender == Gender);
		}

		return query;
	}
}
