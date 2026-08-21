using CAMS.Application.Common.Filtering;
using CAMS.Application.Common.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Application.Common.Repositories;

public interface IRepository<T>
	where T : class
{
	Task<T?> GetByIdAsync(
		Guid id,
		CancellationToken cancellationToken = default);

	Task<PagedResult<T>> SearchAsync<TFilter>(
		PagedRequest<TFilter> request,
		CancellationToken cancellationToken = default)
			where TFilter : class, IFilter<T>;

	Task AddAsync(
		T entity,
		CancellationToken cancellationToken = default);

	Task AddRangeAsync(
		IEnumerable<T> entities,
		CancellationToken cancellationToken = default);

	void Update(T entity);

	void Delete(T entity);

	void DeleteRange(
		IEnumerable<T> entities);
}
