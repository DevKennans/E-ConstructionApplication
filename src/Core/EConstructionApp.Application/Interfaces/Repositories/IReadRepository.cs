using EConstructionApp.Domain.Entities.Common.Abstractions;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace EConstructionApp.Application.Interfaces.Repositories
{
    public interface IReadRepository<T> where T : class, IBaseEntity, new()
    {
        IQueryable<T> Find(Expression<Func<T, bool>> predicate, bool enableTracking = false, bool includeDeleted = false);

        Task<T> GetAsync(Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            bool enableTracking = false, bool includeDeleted = false);

        Task<IList<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            bool enableTracking = false, bool includeDeleted = false);

        Task<IList<T>> GetAllByPagingAsync(Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            bool enableTracking = false, int currentPage = 1, int pageSize = 5, bool includeDeleted = false);

        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, bool includeDeleted = false);
    }
}
