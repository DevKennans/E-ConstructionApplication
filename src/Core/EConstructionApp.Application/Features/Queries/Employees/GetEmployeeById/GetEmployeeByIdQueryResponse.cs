using EConstructionApp.Application.DTOs.Employees;

namespace EConstructionApp.Application.Features.Queries.Employees.GetEmployeeById
{
    public class GetEmployeeByIdQueryResponse
    {
        public bool IsSuccess { get; }
        public string Message { get; }
        public EmployeeDto? Employee { get; }

        public GetEmployeeByIdQueryResponse(bool isSuccess, string message, EmployeeDto? employee)
        {
            IsSuccess = isSuccess;
            Message = message;
            Employee = employee;
        }
    }
}
