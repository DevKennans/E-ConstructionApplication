using EConstructionApp.Application.Interfaces.Services.Entities;
using MediatR;

namespace EConstructionApp.Application.Features.Queries.Employees.GetEmployeeById
{
    public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQueryRequest, GetEmployeeByIdQueryResponse>
    {
        private readonly IEmployeeService _employeeService;
        public GetEmployeeByIdQueryHandler(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<GetEmployeeByIdQueryResponse> Handle(GetEmployeeByIdQueryRequest getEmployeeByIdQueryRequest, CancellationToken cancellationToken)
        {
            (bool isSuccess, string message, DTOs.Employees.EmployeeDto? employee) = await _employeeService.GetEmployeeByIdAsync(getEmployeeByIdQueryRequest.EmployeeId);
            return new GetEmployeeByIdQueryResponse(isSuccess, message, employee);
        }
    }
}
