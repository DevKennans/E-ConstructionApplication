using EConstructionApp.Application.DTOs.Employees;
using EConstructionApp.Application.Features.Commands.Employees.EmployeeCheckInOut;
using EConstructionApp.Application.Features.Queries.Employees.GetEmployeeById;
using EConstructionApp.Application.Interfaces.Services.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EConstructionApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMediator _mediator;
        public EmployeesController(IEmployeeService employeeService, IMediator mediator)
        {
            _employeeService = employeeService;
            _mediator = mediator;
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

        [HttpPost("RecordEmployeeAttendance")]
        public async Task<IActionResult> RecordEmployeeAttendance([FromBody] EmployeeCheckInOutCommandRequest employeeCheckInOutCommandRequest)
        {
            EmployeeCheckInOutCommandResponse employeeCheckInOutCommandResponse = await _mediator.Send(employeeCheckInOutCommandRequest);
            if (!employeeCheckInOutCommandResponse.IsSuccess)
                return BadRequest(new { employeeCheckInOutCommandResponse.IsSuccess, employeeCheckInOutCommandResponse.Message });

            return Ok(new { employeeCheckInOutCommandResponse.IsSuccess, employeeCheckInOutCommandResponse.Message });
        }

        [HttpPost("GetEmployeeById")]
        public async Task<IActionResult> GetEmployeeById([FromQuery] GetEmployeeByIdQueryRequest getEmployeeByIdQueryRequest)
        {
            GetEmployeeByIdQueryResponse getEmployeeByIdQueryResponse = await _mediator.Send(getEmployeeByIdQueryRequest);
            if (!getEmployeeByIdQueryResponse.IsSuccess)
                return BadRequest(new { getEmployeeByIdQueryResponse.IsSuccess, getEmployeeByIdQueryResponse.Message });

            return Ok(new { getEmployeeByIdQueryResponse.IsSuccess, getEmployeeByIdQueryResponse.Message, getEmployeeByIdQueryResponse.Employee });
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
