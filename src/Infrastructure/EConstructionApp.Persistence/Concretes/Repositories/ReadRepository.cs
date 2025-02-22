using EConstructionApp.Application.Interfaces.Repositories;
using EConstructionApp.Domain.Entities.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace EConstructionApp.Persistence.Concretes.Repositories
{
    public class ReadRepository<T> : IReadRepository<T> where T : class, IBaseEntity, new()
    {
        private readonly DbContext _dbContext;
        public ReadRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private DbSet<T> Table { get => _dbContext.Set<T>(); }

        private IQueryable<T> ApplyFilters(IQueryable<T> queryable, bool includeDeleted)
        {
            if (!includeDeleted && typeof(IActivityStatus).IsAssignableFrom(typeof(T)))
                queryable = queryable.Where(e => EF.Property<bool>(e, "IsDeleted") == false);

            return queryable;
        }

        public IQueryable<T> Find(Expression<Func<T, bool>> predicate, bool enableTracking = false, bool includeDeleted = false)
        {
            IQueryable<T> queryable = Table;
            if (!enableTracking) queryable = queryable.AsNoTracking();
            queryable = ApplyFilters(queryable, includeDeleted);
            return queryable.Where(predicate);
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, bool enableTracking = false, bool includeDeleted = false)
        {
            IQueryable<T> queryable = Table;
            if (!enableTracking) queryable = queryable.AsNoTracking();
            queryable = ApplyFilters(queryable, includeDeleted);
            if (include is not null) queryable = include(queryable);

            return await queryable.FirstOrDefaultAsync(predicate);
        }

        public async Task<IList<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, bool enableTracking = false, bool includeDeleted = false)
        {
            IQueryable<T> queryable = Table;
            if (!enableTracking) queryable = queryable.AsNoTracking();
            queryable = ApplyFilters(queryable, includeDeleted);
            if (include is not null) queryable = include(queryable);
            if (predicate is not null) queryable = queryable.Where(predicate);
            if (orderBy is not null)
                return await orderBy(queryable).ToListAsync();

            return await queryable.ToListAsync();
        }

        public async Task<IList<T>> GetAllByPagingAsync(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, bool enableTracking = false, int currentPage = 1, int pageSize = 5, bool includeDeleted = false)
        {
            IQueryable<T> queryable = Table;
            if (!enableTracking) queryable = queryable.AsNoTracking();
            queryable = ApplyFilters(queryable, includeDeleted);
            if (include is not null) queryable = include(queryable);
            if (predicate is not null) queryable = queryable.Where(predicate);
            if (orderBy is not null)
                return await orderBy(queryable).Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();

            return await queryable.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, bool includeDeleted = false)
        {
            IQueryable<T> queryable = Table.AsNoTracking();
            queryable = ApplyFilters(queryable, includeDeleted);
            if (predicate is not null) queryable = queryable.Where(predicate);

            return await queryable.CountAsync();
        }
    }
}
