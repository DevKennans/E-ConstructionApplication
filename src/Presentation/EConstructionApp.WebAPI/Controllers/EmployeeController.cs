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

        [HttpPost("AddEmployee")]
        public async Task<IActionResult> AddEmployee([FromBody] EmployeeInsertDto dto)
        {
            (bool isSuccess, string message) = await _employeeService.InsertEmployeeAsync(dto);

            if (!isSuccess)
                return BadRequest(new { error = message });

            return Ok(new { message });
        }

        [HttpGet("GetAllOrOnlyActiveEmployeesPagedList")]
        public async Task<IActionResult> GetAllOrOnlyActiveEmployeesPagedList([FromQuery] int page, [FromQuery] int size, [FromQuery] bool includeDeleted = false)
        {
            (bool isSuccess, string message, IList<EmployeeDto> employees, int totalEmployees) = await _employeeService.GetAllOrOnlyActiveEmployeesPagedListAsync(page, size, includeDeleted);

            if (!isSuccess || employees == default)
                return NotFound(new { error = message, totalEmployees });

            return Ok(new { message, totalEmployees, employees });
        }
    }
}
