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
    }
}
