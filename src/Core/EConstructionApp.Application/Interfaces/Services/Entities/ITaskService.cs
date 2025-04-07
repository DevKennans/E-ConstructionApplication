using EConstructionApp.Application.DTOs.Tasks;
using EConstructionApp.Application.DTOs.Tasks.Relations;

namespace EConstructionApp.Application.Interfaces.Services.Entities
{
    public interface ITaskService
    {
        Task<(bool IsSuccess, string Message)> InsertAsync(TaskInsertDto? taskInsertDto);

        Task<(bool IsSuccess, string Message)> UpdateTasksDetailsAsync(TaskDetailsUpdateDto? taskDetailsUpdateDto);

        Task<(bool IsSuccess, string Message)> UpdateTasksEmployeesAsync(Guid taskId, List<Guid>? updatedEmployeeIds);

        Task<(bool IsSuccess, string Message)> UpdateTasksMaterialsAsync(Guid taskId, List<MaterialAssignmentInsertDto>? updatedMaterials);

        Task<(bool IsSuccess, string Message, int ActiveTasks, int TotalTasks)> GetBothActiveAndTotalCountsAsync();

        Task<(bool IsSuccess, string Message, TaskDto? Task)> GetEmployeeCurrentTaskAsync(Guid employeeId);

        Task<(bool IsSuccess, string Message, IList<TaskDto>? Tasks)> GetAllActiveTasksListAsync();

        Task<(bool IsSuccess, string Message, IList<TaskDto>? Tasks, int TotalTasks)> GetOnlyActiveTasksPagedListAsync(int pages = 1, int sizes = 5);

        Task<(bool IsSuccess, string Message, IList<TaskStatusCountsDto>? TaskCount)> GetListOfTasksCountsByStatusAsync();

        Task FinishCurrentTaskById(Guid taskId);
        Task<(bool IsSuccess, string Message, Domain.Entities.Task? Task)> GetTaskByIdAsync(Guid taskId);
        Task<(bool IsSuccess, string Message, List<Guid>? EmployeeIds)> GetTaskEmployeeIdsAsync(Guid taskId);

    }
}
