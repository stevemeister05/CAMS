using CAMS.Application.Common.Filtering;
using CAMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Application.Event.DTOs;

public class EventFilter : IFilter<Domain.Entities.Event>
{
	public string? Search { get; set; }

	public EventType? Type { get; set; }

	public EventStatus? Status { get; set; }

	public DateOnly? EventDateFrom { get; set; }

	public DateOnly? EventDateTo { get; set; }

	public IQueryable<Domain.Entities.Event> Filter(
		IQueryable<Domain.Entities.Event> query)
	{
		if (!string.IsNullOrWhiteSpace(Search))
		{
			var search = Search.Trim();

			query = query.Where(x =>
				x.Name.Contains(search));
		}

		if (Type.HasValue)
		{
			query = query.Where(x =>
				x.Type == Type.Value);
		}

		if (Status.HasValue)
		{
			query = query.Where(x =>
				x.Status == Status.Value);
		}

		if (EventDateFrom.HasValue)
		{
			query = query.Where(x =>
				x.EventDate >= EventDateFrom.Value);
		}

		if (EventDateTo.HasValue)
		{
			query = query.Where(x =>
				x.EventDate <= EventDateTo.Value);
		}

		return query;
	}
}
