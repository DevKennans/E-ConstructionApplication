using EConstructionApp.Application.DTOs.Employees;

namespace EConstructionApp.Application.Interfaces.Services.Entities
{
    public interface IEmployeeService
    {
        Task<(bool IsSuccess, string Message)> InsertAsync(EmployeeInsertDto dto);

        Task<(bool IsSuccess, string Message)> UpdateAsync(EmployeeUpdateDto dto);

        Task<(bool IsSuccess, string Message)> SafeDeleteEmployeeAsync(Guid employeeId);

        Task<(bool IsSuccess, string Message)> RestoreEmployeeAsync(Guid employeeId);

        Task<(bool IsSuccess, string Message, int ActiveEmployees, int TotalEmployees)> GetEmployeeCountsAsync();

        Task<(bool IsSuccess, string Message, IList<EmployeeDto> Employees)> GetAvailableEmployeesListAsync();

        Task<(bool IsSuccess, string Message, IList<EmployeeDto> Employees, int TotalEmployees)> GetAllOrOnlyActiveEmployeesPagedListAsync(int page = 1, int size = 5, bool includeDeleted = false);

        Task<(bool IsSuccess, string Message, IList<EmployeeDto> Employees, int TotalDeletedEmployees)> GetDeletedEmployeesPagedListAsync(int page = 1, int size = 5);
    }
}
