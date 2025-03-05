using EConstructionApp.Application.DTOs.Tasks;
using EConstructionApp.Application.DTOs.Tasks.Relations;

namespace EConstructionApp.Application.Interfaces.Services.Entities
{
    public interface ITaskService
    {
        Task<(bool IsSuccess, string Message)> InsertAsync(TaskInsertDto? taskInsertDto);

        Task<(bool IsSuccess, string Message)> UpdateTaskDetailsAsync(TaskDetailsUpdateDto? taskDetailsUpdateDto);

        Task<(bool IsSuccess, string Message)> UpdateTaskEmployeesAsync(Guid taskId, List<Guid> updatedEmployeeIds);

        Task<(bool IsSuccess, string Message)> UpdateTaskMaterialsAsync(Guid taskId, List<MaterialAssignmentInsertDto>? updatedMaterials);

        Task<(bool IsSuccess, string Message, int ActiveTasks, int TotalTasks)> GetTasksCountsAsync();

        Task<(bool IsSuccess, string Message, TaskDto? Task)> GetEmployeeCurrentTaskAsync(Guid employeeId);

        Task<(bool IsSuccess, string Message, IList<TaskDto>? Tasks)> GetAllActiveTaskListAsync();

        Task<(bool IsSuccess, string Message, IList<TaskStatusCountsDto>? TaskCount)> GetListOfTaskCountByStatusAsync();
    }
}
