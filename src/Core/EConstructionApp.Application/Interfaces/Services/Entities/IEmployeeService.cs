using EConstructionApp.Application.DTOs.Employees;

namespace EConstructionApp.Application.Interfaces.Services.Entities
{
    public interface IEmployeeService
    {
        Task<(bool isSuccess, string message)> InsertEmployeeAsync(EmployeeInsertDto dto);

        Task<(bool isSuccess, string message, IList<EmployeeDto> employees, int totalEmployees)> GetAllOrOnlyActiveEmployeesPagedListAsync(int page = 1, int size = 5, bool includeDeleted = false);
    }
}
