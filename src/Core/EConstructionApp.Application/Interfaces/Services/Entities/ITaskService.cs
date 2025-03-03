using EConstructionApp.Application.DTOs.Tasks;

namespace EConstructionApp.Application.Interfaces.Services.Entities
{
    public interface ITaskService
    {
        Task<(bool IsSuccess, string Message)> InsertAsync(TaskInsertDto dto);

        Task<(bool IsSuccess, string Message, int ActiveTasks, int TotalTasks)> GetTaskCountsAsync();

        Task<(bool IsSuccess, string Message, IList<TaskDto> Tasks)> GetAllActiveTasksListAsync();
    }
}
