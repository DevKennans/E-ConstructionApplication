using EConstructionApp.Application.DTOs.Tasks;
using EConstructionApp.Application.DTOs.Tasks.Relations;
using EConstructionApp.Application.Interfaces.Services.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EConstructionApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpPost]
        public async Task<IActionResult> InsertTasks(TaskInsertDto taskInsertDtodto)
        {
            (bool IsSuccess, string Message) = await _taskService.InsertAsync(taskInsertDtodto);
            if (!IsSuccess)
                return BadRequest(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message });
        }

        [HttpPut("UpdateTasksDetails")]
        public async Task<IActionResult> UpdateTasksDetails([FromBody] TaskDetailsUpdateDto taskDetailsUpdateDtodto)
        {
            (bool IsSuccess, string Message) = await _taskService.UpdateTaskDetailsAsync(taskDetailsUpdateDtodto);
            if (!IsSuccess)
                return BadRequest(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message });
        }

        [HttpPut("UpdateTaskEmployees")]
        public async Task<IActionResult> UpdateTaskEmployees(Guid taskId, [FromBody] List<Guid> updatedEmployeeIds)
        {
            (bool IsSuccess, string Message) = await _taskService.UpdateTaskEmployeesAsync(taskId, updatedEmployeeIds);
            if (!IsSuccess)
                return BadRequest(new { Success = false, Message });

            return Ok(new { Success = true, Message });
        }

        [HttpPut("UpdateTaskMaterials")]
        public async Task<IActionResult> UpdateTaskMaterials(Guid taskId, [FromBody] List<MaterialAssignmentInsertDto> updatedMaterials)
        {
            (bool IsSuccess, string Message) = await _taskService.UpdateTaskMaterialsAsync(taskId, updatedMaterials);
            if (!IsSuccess)
                return BadRequest(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message });
        }

        [HttpGet("GetTasksCounts")]
        public async Task<IActionResult> GetTasksCounts()
        {
            (bool IsSuccess, string Message, int ActiveTasks, int TotalTasks) = await _taskService.GetTasksCountsAsync();
            if (!IsSuccess)
                return NotFound(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message, ActiveTasks, TotalTasks });
        }

        [HttpGet("GetEmployeeCurrentTask/{employeeId}")]
        public async Task<IActionResult> GetEmployeeCurrentTask(Guid employeeId)
        {
            (bool IsSuccess, string Message, TaskDto? Task) = await _taskService.GetEmployeeCurrentTaskAsync(employeeId);
            if (!IsSuccess)
                return NotFound(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message, Task });
        }

        [HttpGet("GetAllActiveTaskList")]
        public async Task<IActionResult> GetAllActiveTaskList()
        {
            (bool IsSuccess, string Message, IList<TaskDto>? Tasks) = await _taskService.GetAllActiveTaskListAsync();
            if (!IsSuccess)
                return NotFound(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message, Tasks });
        }

        [HttpGet("GetListOfTaskCountByStatus")]
        public async Task<IActionResult> GetListOfTaskCountByStatus()
        {
            (bool IsSuccess, string Message, IList<Application.DTOs.Tasks.Relations.TaskStatusCountsDto>? TaskCounts) = await _taskService.GetListOfTaskCountByStatusAsync();
            if (!IsSuccess)
                return NotFound(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message, TaskCounts });
        }
    }
}
