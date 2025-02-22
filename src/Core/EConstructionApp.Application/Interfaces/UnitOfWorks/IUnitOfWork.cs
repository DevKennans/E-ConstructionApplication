using EConstructionApp.Application.Interfaces.Repositories;
using EConstructionApp.Domain.Entities.Common.Abstractions;

namespace EConstructionApp.Application.Interfaces.UnitOfWorks
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IReadRepository<T> GetReadRepository<T>() where T : class, IBaseEntity, new();
        IWriteRepository<T> GetWriteRepository<T>() where T : class, IBaseEntity, new();
        Task<int> SaveAsync();
        int Save();
    }
}
