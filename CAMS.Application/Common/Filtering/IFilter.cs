using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace CAMS.Application.Common.Filtering;

public interface IFilter<T>
	where T : class
{
	IQueryable<T> Filter(IQueryable<T> query);
}
