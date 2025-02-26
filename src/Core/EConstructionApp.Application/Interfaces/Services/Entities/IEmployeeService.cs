using EConstructionApp.Application.DTOs.Employees;

namespace EConstructionApp.Application.Interfaces.Services.Entities
{
    public interface IEmployeeService
    {
        Task<(bool isSuccess, string message)> InsertEmployeeAsync(EmployeeInsertDto dto);
    }
}
