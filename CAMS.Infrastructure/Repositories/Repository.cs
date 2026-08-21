using CAMS.Application.Common.Filtering;
using CAMS.Application.Common.Pagination;
using CAMS.Application.Common.Repositories;
using CAMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CAMS.Infrastructure.Repositories;

public class Repository<T> : IRepository<T>
	where T : class
{
	protected readonly CAMSDBContext Context;
	protected readonly DbSet<T> DbSet;

	public Repository(CAMSDBContext context)
	{
		Context = context;
		DbSet = context.Set<T>();
	}

	public virtual async Task<T?> GetByIdAsync(
		Guid id,
		CancellationToken cancellationToken = default)
	{
		return await DbSet.FindAsync(
			new object[] { id },
			cancellationToken);
	}

	public async Task AddRangeAsync(
		IEnumerable<T> entities,
		CancellationToken cancellationToken = default)
	{
		await DbSet.AddRangeAsync(
			entities,
			cancellationToken);
	}

	public virtual async Task<PagedResult<T>> SearchAsync<TFilter>(
		PagedRequest<TFilter> request,
		CancellationToken cancellationToken = default)
			where TFilter : class, IFilter<T>
	{
		var query = DbSet.AsNoTracking();

		if (request.Filter is not null)
		{
			query = request.Filter.Filter(query);
		}

		var totalCount = await query.CountAsync(
			cancellationToken);

		var items = await query
			.Skip((request.Page - 1) * request.PageSize)
			.Take(request.PageSize)
			.ToListAsync(cancellationToken);

		return new PagedResult<T>(
			items,
			totalCount,
			request.Page,
			request.PageSize);
	}

	public virtual async Task AddAsync(
		T entity,
		CancellationToken cancellationToken = default)
	{
		await DbSet.AddAsync(
			entity,
			cancellationToken);
	}

	public virtual void Update(T entity)
	{
		DbSet.Update(entity);
	}

	public virtual void Delete(T entity)
	{
		DbSet.Remove(entity);
	}

	public virtual void DeleteRange(
		IEnumerable<T> entities)
	{
		DbSet.RemoveRange(entities);
	}
}
