using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Application.Common.Pagination;

public class PagedRequest<TFilter>
{
	public int Page { get; set; } = 1;

	public int PageSize { get; set; } = 20;

	public TFilter? Filter { get; set; }

	public string? SortBy { get; set; }

	public bool SortDescending { get; set; }
}
