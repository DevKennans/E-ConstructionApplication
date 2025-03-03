using EConstructionApp.Application.DTOs.Tasks;
using EConstructionApp.Application.Interfaces.Services.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EConstructionApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpPost("InsertTask")]
        public async Task<IActionResult> InsertTask(TaskInsertDto dto)
        {
            if (dto is null)
                return BadRequest("Invalid task data.");

            (bool IsSuccess, string Message) = await _taskService.InsertAsync(dto);
            if (!IsSuccess)
                return BadRequest(new { Success = false, Message });

            return Ok(new { Success = true, Message });
        }

        [HttpPut("UpdateMaterial")]
        public async Task<IActionResult> UpdateTaskDetails([FromBody] TaskDetailsUpdateDto dto)
        {
            if (dto is null)
                return BadRequest("Invalid request data.");

            (bool IsSuccess, string Message) = await _taskService.UpdateTaskDetailsAsync(dto);
            if (!IsSuccess)
                return BadRequest(Message);

            return Ok(Message);
        }

        [HttpGet("GetTaskCounts")]
        public async Task<IActionResult> GetTaskCounts()
        {
            var (IsSuccess, Message, ActiveTasks, TotalTasks) = await _taskService.GetTaskCountsAsync();

            if (!IsSuccess)
                return NotFound(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message, ActiveTasks, TotalTasks });
        }

        [HttpGet("GetAllActiveTasksList")]
        public async Task<IActionResult> GetAllActiveTasksList()
        {
            (bool IsSuccess, string Message, IList<TaskDto> Tasks) = await _taskService.GetAllActiveTasksListAsync();
            if (!IsSuccess)
                return NotFound(new { Message });

            return Ok(new { Message, Tasks });
        }
    }
}
