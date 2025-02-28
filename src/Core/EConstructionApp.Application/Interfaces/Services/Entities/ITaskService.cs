using EConstructionApp.Application.DTOs.Tasks;

namespace EConstructionApp.Application.Interfaces.Services.Entities
{
    public interface ITaskService
    {
        Task<(bool IsSuccess, string Message)> InsertAsync(TaskInsertDto dto);
    }
}
