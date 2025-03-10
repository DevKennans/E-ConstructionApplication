using EConstructionApp.Application.DTOs.Employees;
using EConstructionApp.Application.Interfaces.Services.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EConstructionApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Admin")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpPost("InsertEmployees")]
        public async Task<IActionResult> InsertEmployees([FromBody] EmployeeInsertDto? employeeInsertDto)
        {
            (bool IsSuccess, string Message) = await _employeeService.InsertAsync(employeeInsertDto);
            if (!IsSuccess)
                return BadRequest(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message });
        }

        [HttpPut("UpdateEmployees")]
        public async Task<IActionResult> UpdateEmployees([FromBody] EmployeeUpdateDto? employeeUpdateDto)
        {
            (bool IsSuccess, string Message) = await _employeeService.UpdateAsync(employeeUpdateDto);
            if (!IsSuccess)
                return BadRequest(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message });
        }

        [HttpDelete("SafeDeleteEmployees/{employeeId}")]
        public async Task<IActionResult> SafeDeleteEmployees(Guid employeeId)
        {
            (bool IsSuccess, string Message) = await _employeeService.SafeDeleteAsync(employeeId);
            if (!IsSuccess)
                return BadRequest(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message });
        }

        [HttpPut("RestoreEmployees/{employeeId}")]
        public async Task<IActionResult> RestoreEmployees(Guid employeeId)
        {
            (bool IsSuccess, string Message) = await _employeeService.RestoreDeletedAsync(employeeId);
            if (!IsSuccess)
                return BadRequest(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message });
        }

        [HttpGet("GetEmployeesCounts")]
        public async Task<IActionResult> GetEmployeesCounts()
        {
            (bool IsSuccess, string Message, int ActiveEmployees, int TotalEmployees) = await _employeeService.GetBothActiveAndTotalCountsAsync();
            if (!IsSuccess)
                return NotFound(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message, ActiveEmployees, TotalEmployees });
        }

        [HttpGet("GetAvailableEmployeesList")]
        public async Task<IActionResult> GetAvailableEmployeesList()
        {
            (bool IsSuccess, string Message, IList<EmployeeDto>? Employees) = await _employeeService.GetAvailableEmployeesListAsync();
            if (!IsSuccess)
                return NotFound(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message, Employees });
        }

        [HttpGet("GetOnlyActiveEmployeesPagedList")]
        public async Task<IActionResult> GetOnlyActiveEmployeesPagedList([FromQuery] int pages = 1, [FromQuery] int sizes = 5)
        {
            (bool IsSuccess, string Message, IList<EmployeeDto>? Employees, int TotalEmployees) = await _employeeService.GetOnlyActiveEmployeesPagedListAsync(pages, sizes);
            if (!IsSuccess)
                return NotFound(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message, TotalEmployees, Employees });
        }

        [HttpGet("GetDeletedEmployeesPagedList")]
        public async Task<IActionResult> GetDeletedEmployeesPagedList([FromQuery] int pages = 1, [FromQuery] int sizes = 5)
        {
            (bool IsSuccess, string Message, IList<EmployeeDto>? Employees, int TotalDeletedEmployees) = await _employeeService.GetDeletedEmployeesPagedListAsync(pages, sizes);
            if (!IsSuccess)
                return NotFound(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message, TotalDeletedEmployees, Employees });
        }
    }
}
