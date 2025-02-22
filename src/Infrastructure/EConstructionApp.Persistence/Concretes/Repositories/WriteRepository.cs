using EConstructionApp.Application.Interfaces.Repositories;
using EConstructionApp.Domain.Entities.Common.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace EConstructionApp.Persistence.Concretes.Repositories
{
    public class WriteRepository<T> : IWriteRepository<T> where T : class, IBaseEntity, new()
    {
        private readonly DbContext _dbContext;
        public WriteRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private DbSet<T> Table { get => _dbContext.Set<T>(); }

        public async Task AddAsync(T entity)
        {
            await Table.AddAsync(entity);
        }

        public async Task AddRangeAsync(IList<T> entities)
        {
            await Table.AddRangeAsync(entities);
        }

        public async Task<T> UpdateAsync(T entity)
        {
            await Task.Run(() => Table.Update(entity));
            return entity;
        }

        public async Task HardDeleteAsync(T entity)
        {
            await Task.Run(() => Table.Remove(entity));
        }

        public async Task HardDeleteRangeAsync(IList<T> entity)
        {
            await Task.Run(() => Table.RemoveRange(entity));
        }
    }
}
