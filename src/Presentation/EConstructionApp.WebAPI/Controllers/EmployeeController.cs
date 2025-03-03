using EConstructionApp.Application.DTOs.Employees;
using EConstructionApp.Application.Interfaces.Services.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EConstructionApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpPost("InsertEmployee")]
        public async Task<IActionResult> InsertEmployee([FromBody] EmployeeInsertDto dto)
        {
            (bool IsSuccess, string Message) = await _employeeService.InsertAsync(dto);
            if (!IsSuccess)
                return BadRequest(new { Message });

            return Ok(new { Message });
        }

        [HttpPut("UpdateEmployee")]
        public async Task<IActionResult> UpdateEmployee([FromBody] EmployeeUpdateDto dto)
        {
            (bool IsSuccess, string Message) = await _employeeService.UpdateAsync(dto);
            if (!IsSuccess)
                return BadRequest(new { Message });

            return Ok(new { Message });
        }

        [HttpDelete("SafeDeleteEmployee/{employeeId}")]
        public async Task<IActionResult> SafeDeleteEmployee(Guid employeeId)
        {
            (bool IsSuccess, string Message) = await _employeeService.SafeDeleteEmployeeAsync(employeeId);
            if (!IsSuccess)
                return BadRequest(new { Message });

            return Ok(new { Message });
        }

        [HttpPut("RestoreEmployee/{employeeId}")]
        public async Task<IActionResult> RestoreEmployee(Guid employeeId)
        {
            (bool IsSuccess, string Message) = await _employeeService.RestoreEmployeeAsync(employeeId);
            if (!IsSuccess)
                return BadRequest(new { Message });

            return Ok(new { Message });
        }

        [HttpGet("GetEmployeeCounts")]
        public async Task<IActionResult> GetEmployeeCounts()
        {
            (bool IsSuccess, string Message, int ActiveEmployees, int TotalEmployees) = await _employeeService.GetEmployeeCountsAsync();
            if (!IsSuccess)
                return NotFound(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message, ActiveEmployees, TotalEmployees });
        }

        [HttpGet("GetAvailableEmployeesList")]
        public async Task<IActionResult> GetAvailableEmployeesList()
        {
            (bool IsSuccess, string Message, IList<EmployeeDto> Employees) = await _employeeService.GetAvailableEmployeesListAsync();
            if (!IsSuccess)
                return NotFound(new { Message });

            return Ok(new { Message, Employees });
        }

        [HttpGet("GetAllOrOnlyActiveEmployeesPagedList")]
        public async Task<IActionResult> GetAllOrOnlyActiveEmployeesPagedList([FromQuery] int page, [FromQuery] int size, [FromQuery] bool IncludeDeleted = false)
        {
            (bool IsSuccess, string Message, IList<EmployeeDto> Employees, int TotalEmployees) = await _employeeService.GetAllOrOnlyActiveEmployeesPagedListAsync(page, size, IncludeDeleted);
            if (!IsSuccess || Employees == default)
                return NotFound(new { Message, TotalEmployees });

            return Ok(new { Message, TotalEmployees, Employees });
        }

        [HttpGet("GetDeletedEmployeesPagedList")]
        public async Task<IActionResult> GetDeletedEmployeesPagedList([FromQuery] int page, [FromQuery] int size)
        {
            (bool IsSuccess, string Message, IList<EmployeeDto> Employees, int TotalDeletedEmployees) = await _employeeService.GetDeletedEmployeesPagedListAsync(page, size);
            if (!IsSuccess)
                return NotFound(new { Message, TotalDeletedEmployees });

            return Ok(new { Message, TotalDeletedEmployees, Employees });
        }
    }
}
