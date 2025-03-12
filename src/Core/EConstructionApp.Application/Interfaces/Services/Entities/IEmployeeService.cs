using EConstructionApp.Application.DTOs.Employees;

namespace EConstructionApp.Application.Interfaces.Services.Entities
{
    public interface IEmployeeService
    {
        Task<(bool IsSuccess, string Message)> InsertAsync(EmployeeInsertDto? employeeInsertDto);

        Task<(bool IsSuccess, string Message)> UpdateAsync(EmployeeUpdateDto? employeeUpdateDto);

        Task<(bool IsSuccess, string Message)> SafeDeleteAsync(Guid employeeId);

        Task<(bool IsSuccess, string Message)> RestoreDeletedAsync(Guid employeeId);

        Task<(bool IsSuccess, string Message, int ActiveEmployees, int TotalEmployees)> GetBothActiveAndTotalCountsAsync();

        Task<(bool IsSuccess, string Message, EmployeeDto? employee)> GetEmployeeByIdAsync(Guid employeeId);

        Task<(bool IsSuccess, string Message, IList<EmployeeDto>? Employees)> GetAvailableEmployeesListAsync();

        Task<(bool IsSuccess, string Message, IList<EmployeeDto>? Employees, int TotalEmployees)> GetOnlyActiveEmployeesPagedListAsync(int pages = 1, int sizes = 5);

        Task<(bool IsSuccess, string Message, IList<EmployeeDto>? Employees, int TotalDeletedEmployees)> GetDeletedEmployeesPagedListAsync(int pages = 1, int sizes = 5);
    }
}
