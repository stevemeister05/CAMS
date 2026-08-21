using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Application.Common.Pagination;

public class PagedResult<T>
{
	public IReadOnlyList<T> Items { get; }

	public int TotalCount { get; }

	public int Page { get; }

	public int PageSize { get; }

	public int TotalPages =>
		PageSize <= 0
			? 0
			: (int)Math.Ceiling(
				(double)TotalCount / PageSize);

	public bool HasPreviousPage =>
		Page > 1;

	public bool HasNextPage =>
		Page < TotalPages;

	public PagedResult(
		IReadOnlyList<T> items,
		int totalCount,
		int page,
		int pageSize)
	{
		Items = items;
		TotalCount = totalCount;
		Page = page;
		PageSize = pageSize;
	}
}
