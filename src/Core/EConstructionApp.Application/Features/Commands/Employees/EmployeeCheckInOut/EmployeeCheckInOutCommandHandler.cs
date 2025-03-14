using EConstructionApp.Application.Interfaces.Services.Entities;
using MediatR;

namespace EConstructionApp.Application.Features.Commands.Employees.EmployeeCheckInOut
{
    public class EmployeeCheckInOutCommandHandler : IRequestHandler<EmployeeCheckInOutCommandRequest, EmployeeCheckInOutCommandResponse>
    {
        private readonly IEmployeeService _employeeService;
        public EmployeeCheckInOutCommandHandler(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<EmployeeCheckInOutCommandResponse> Handle(EmployeeCheckInOutCommandRequest employeeCheckInOutCommandRequest, CancellationToken cancellationToken)
        {
            (bool IsSuccess, string Message) handlerResponse = await _employeeService.RecordAttendanceAsync(employeeCheckInOutCommandRequest.EmployeeId, employeeCheckInOutCommandRequest.ScanTime);

            return new EmployeeCheckInOutCommandResponse
            {
                IsSuccess = handlerResponse.IsSuccess,
                Message = handlerResponse.Message
            };
        }
    }
}
