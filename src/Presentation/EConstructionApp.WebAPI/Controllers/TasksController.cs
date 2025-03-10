using EConstructionApp.Application.DTOs.Tasks;
using EConstructionApp.Application.DTOs.Tasks.Relations;
using EConstructionApp.Application.Interfaces.Services.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EConstructionApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Admin")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpPost("InsertTasks")]
        public async Task<IActionResult> InsertTasks(TaskInsertDto? taskInsertDtodto)
        {
            (bool IsSuccess, string Message) = await _taskService.InsertAsync(taskInsertDtodto);
            if (!IsSuccess)
                return BadRequest(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message });
        }

        [HttpPut("UpdateTasksDetails")]
        public async Task<IActionResult> UpdateTasksDetails([FromBody] TaskDetailsUpdateDto? taskDetailsUpdateDtodto)
        {
            (bool IsSuccess, string Message) = await _taskService.UpdateTasksDetailsAsync(taskDetailsUpdateDtodto);
            if (!IsSuccess)
                return BadRequest(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message });
        }

        [HttpPut("UpdateTasksEmployees")]
        public async Task<IActionResult> UpdateTasksEmployees(Guid taskId, [FromBody] List<Guid>? updatedEmployeeIds)
        {
            (bool IsSuccess, string Message) = await _taskService.UpdateTasksEmployeesAsync(taskId, updatedEmployeeIds!);
            if (!IsSuccess)
                return BadRequest(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message });
        }

        [HttpPut("UpdateTasksMaterials")]
        public async Task<IActionResult> UpdateTasksMaterials(Guid taskId, [FromBody] List<MaterialAssignmentInsertDto> updatedMaterials)
        {
            (bool IsSuccess, string Message) = await _taskService.UpdateTasksMaterialsAsync(taskId, updatedMaterials);
            if (!IsSuccess)
                return BadRequest(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message });
        }

        [HttpGet("GetTasksCounts")]
        public async Task<IActionResult> GetTasksCounts()
        {
            (bool IsSuccess, string Message, int ActiveTasks, int TotalTasks) = await _taskService.GetBothActiveAndTotalCountsAsync();
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

        [HttpGet("GetAllActiveTasksList")]
        public async Task<IActionResult> GetAllActiveTasksList()
        {
            (bool IsSuccess, string Message, IList<TaskDto>? Tasks) = await _taskService.GetAllActiveTasksListAsync();
            if (!IsSuccess)
                return NotFound(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message, Tasks });
        }

        [HttpGet("GetOnlyActiveTasksPagedList")]
        public async Task<IActionResult> GetOnlyActiveTasksPagedList([FromQuery] int pages = 1, [FromQuery] int sizes = 5)
        {
            (bool IsSuccess, string Message, IList<TaskDto>? Tasks, int TotalTasks) = await _taskService.GetOnlyActiveTasksPagedListAsync(pages, sizes);
            if (!IsSuccess)
                return NotFound(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message, Tasks, TotalTasks });
        }

        [HttpGet("GetListOfTasksCountsByStatus")]
        public async Task<IActionResult> GetListOfTasksCountsByStatus()
        {
            (bool IsSuccess, string Message, IList<TaskStatusCountsDto>? TaskCounts) = await _taskService.GetListOfTasksCountsByStatusAsync();
            if (!IsSuccess)
                return NotFound(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message, TaskCounts });
        }
    }
}
