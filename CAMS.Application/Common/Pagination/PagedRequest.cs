using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Application.Common.Pagination;

public class PagedRequest<TFilter>
{
	public int Page { get; set; } = 1;

	public int PageSize { get; set; } = 20;

	public TFilter? Filter { get; set; }

	public Sort[]? SortBy { get; set; }
}

public class Sort
{
	public string Name { get; set; }

	public bool SortDescending { get; set; } = false;
}