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
        public async Task<IActionResult> InsertEmployee([FromBody] EmployeeInsertDto Dto)
        {
            (bool IsSuccess, string Message) = await _employeeService.InsertAsync(Dto);

            if (!IsSuccess)
                return BadRequest(new { error = Message });

            return Ok(new { Message });
        }

        [HttpGet("GetAvailableEmployeesList")]
        public async Task<IActionResult> GetAvailableEmployeesList()
        {
            (bool IsSuccess, string Message, IList<EmployeeDto> Employees) = await _employeeService.GetAvailableEmployeesListAsync();

            if (!IsSuccess)
                return NotFound(new { error = Message });

            return Ok(new { Message, Employees });
        }

        [HttpGet("GetAllOrOnlyActiveEmployeesPagedList")]
        public async Task<IActionResult> GetAllOrOnlyActiveEmployeesPagedList([FromQuery] int Page, [FromQuery] int Size, [FromQuery] bool IncludeDeleted = false)
        {
            (bool IsSuccess, string Message, IList<EmployeeDto> Employees, int TotalEmployees) = await _employeeService.GetAllOrOnlyActiveEmployeesPagedListAsync(Page, Size, IncludeDeleted);

            if (!IsSuccess || Employees == default)
                return NotFound(new { error = Message, TotalEmployees });

            return Ok(new { Message, TotalEmployees, Employees });
        }

        [HttpGet("GetDeletedEmployeesPagedList")]
        public async Task<IActionResult> GetDeletedEmployeesPagedList([FromQuery] int Page, [FromQuery] int Size)
        {
            (bool IsSuccess, string Message, IList<EmployeeDto> Employees, int TotalDeletedEmployees) = await _employeeService.GetDeletedEmployeesPagedListAsync(Page, Size);

            if (!IsSuccess)
                return NotFound(new { error = Message, TotalDeletedEmployees });

            return Ok(new { Message, TotalDeletedEmployees, Employees });
        }

        [HttpPut("UpdateEmployee")]
        public async Task<IActionResult> UpdateEmployee([FromBody] EmployeeUpdateDto Dto)
        {
            (bool IsSuccess, string? Message) = await _employeeService.UpdateAsync(Dto);

            if (!IsSuccess)
                return BadRequest(new { error = Message });

            return Ok(new { Message });
        }

        [HttpDelete("SafeDeleteEmployee/{employeeId}")]
        public async Task<IActionResult> SafeDeleteEmployee(Guid employeeId)
        {
            (bool isSuccess, string message) = await _employeeService.SafeDeleteEmployeeAsync(employeeId);

            if (!isSuccess)
                return BadRequest(new { message });

            return Ok(new { message });
        }

        [HttpPut("RestoreEmployee/{employeeId}")]
        public async Task<IActionResult> RestoreEmployee(Guid employeeId)
        {
            (bool isSuccess, string message) = await _employeeService.RestoreEmployeeAsync(employeeId);

            if (!isSuccess)
                return BadRequest(new { message });

            return Ok(new { message });
        }
    }
}
