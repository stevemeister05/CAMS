using CAMS.Application.Common.Filtering;
using CAMS.Domain.Enums;

namespace CAMS.Application.EventSchedule.DTOs;

public class EventScheduleFilter : IFilter<Domain.Entities.EventSchedule>
{
	public string? Search { get; set; }

	public EventType? EventType { get; set; }

	public bool? IsActive { get; set; }

	public IQueryable<Domain.Entities.EventSchedule> Filter(
		IQueryable<Domain.Entities.EventSchedule> query)
	{
		if (!string.IsNullOrWhiteSpace(Search))
		{
			var search = Search.Trim();

			query = query.Where(x =>
				x.Name.Contains(search));
		}

		if (EventType.HasValue)
		{
			query = query.Where(x =>
				x.EventType == EventType.Value);
		}

		if (IsActive.HasValue)
		{
			query = query.Where(x =>
				x.IsActive == IsActive.Value);
		}

		return query;
	}
}
