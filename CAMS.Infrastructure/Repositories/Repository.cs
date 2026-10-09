using CAMS.Application.Common.Filtering;
using CAMS.Application.Common.Pagination;
using CAMS.Application.Common.Repositories;
using CAMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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

		query = ApplySorting(
			query,
			request.SortBy);

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

	protected virtual IQueryable<T> ApplySorting(
	IQueryable<T> query,
	IReadOnlyList<Sort>? sorts)
	{
		if (
			sorts is null ||
			sorts.Count == 0
		)
		{
			return ApplyDefaultSorting(
				query);
		}


		var appliedSort =
			false;


		foreach (
			var sort in sorts)
		{
			if (
				string.IsNullOrWhiteSpace(
					sort.Name)
			)
			{
				continue;
			}


			var property =
				typeof(T)
					.GetProperties()
					.FirstOrDefault(
						x =>
							string.Equals(
								x.Name,
								sort.Name,
								StringComparison.OrdinalIgnoreCase));


			if (property is null)
			{
				continue;
			}


			var parameter =
				Expression.Parameter(
					typeof(T),
					"x");


			var propertyAccess =
				Expression.Property(
					parameter,
					property);


			var keySelector =
				Expression.Lambda(
					propertyAccess,
					parameter);


			string methodName;


			if (!appliedSort)
			{
				methodName =
					sort.SortDescending
						? nameof(
							Queryable.OrderByDescending)
						: nameof(
							Queryable.OrderBy);
			}
			else
			{
				methodName =
					sort.SortDescending
						? nameof(
							Queryable.ThenByDescending)
						: nameof(
							Queryable.ThenBy);
			}


			var expression =
				Expression.Call(
					typeof(Queryable),
					methodName,
					[
						typeof(T),
					property.PropertyType
					],
					query.Expression,
					Expression.Quote(
						keySelector));


			query =
				query.Provider
					.CreateQuery<T>(
						expression);


			appliedSort =
				true;
		}


		return appliedSort
			? query
			: ApplyDefaultSorting(
				query);
	}

	protected virtual IQueryable<T> ApplyDefaultSorting(
		IQueryable<T> query)
	{
		var idProperty =
			typeof(T)
				.GetProperties()
				.FirstOrDefault(
					x =>
						string.Equals(
							x.Name,
							"Id",
							StringComparison.OrdinalIgnoreCase));


		if (idProperty is null)
		{
			return query;
		}


		return ApplySorting(
			query,
			[
				new Sort
			{
				Name =
					idProperty.Name,

				SortDescending =
					false
			}
			]);
	}
}
