using EConstructionApp.Application.Interfaces.Repositories;
using EConstructionApp.Application.Interfaces.UnitOfWorks;
using EConstructionApp.Persistence.Concretes.Repositories;
using EConstructionApp.Persistence.Contexts;

namespace EConstructionApp.Persistence.Concretes.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EConstructionDbContext _dbContext;
        public UnitOfWork(EConstructionDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async ValueTask DisposeAsync() => await _dbContext.DisposeAsync();

        public int Save() => _dbContext.SaveChanges();
        public async Task<int> SaveAsync() => await _dbContext.SaveChangesAsync();
        IReadRepository<T> IUnitOfWork.GetReadRepository<T>() => new ReadRepository<T>(_dbContext);
        IWriteRepository<T> IUnitOfWork.GetWriteRepository<T>() => new WriteRepository<T>(_dbContext);
    }
}
