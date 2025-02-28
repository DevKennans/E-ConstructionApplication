using AutoMapper;
using EConstructionApp.Application.DTOs.Employees;
using EConstructionApp.Application.Interfaces.Services.Entities;
using EConstructionApp.Application.Interfaces.UnitOfWorks;
using EConstructionApp.Domain.Entities;

namespace EConstructionApp.Persistence.Concretes.Services.Entities
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<(bool IsSuccess, string Message)> InsertAsync(EmployeeInsertDto dto)
        {
            if (dto is null)
                return (false, "Invalid employee data.");

            List<string> validationErrors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.FirstName))
                validationErrors.Add("First name cannot be empty.");
            if (string.IsNullOrWhiteSpace(dto.LastName))
                validationErrors.Add("Last name cannot be empty.");

            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (dto.DateOfBirth > today.AddYears(-18))
                validationErrors.Add("Employee must be at least 18 years old.");

            if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
                validationErrors.Add("Phone number is required.");
            if (string.IsNullOrWhiteSpace(dto.Address))
                validationErrors.Add("Address is required.");
            if (dto.Salary <= 0)
                validationErrors.Add("Salary must be greater than zero.");

            if (validationErrors.Any())
                return (false, string.Join(" ", validationErrors));

            Employee employee = _mapper.Map<Employee>(dto);

            await _unitOfWork.GetWriteRepository<Employee>().AddAsync(employee);
            await _unitOfWork.SaveAsync();

            return (true, $"Employee '{dto.FirstName} {dto.LastName}' has been successfully added.");
        }

        /* GetAvailableEmployeesListAsync method retrieves all non-deleted employees who are currently not assigned to any task. */
        public async Task<(bool IsSuccess, string Message, IList<EmployeeDto> Employees)> GetAvailableEmployeesListAsync()
        {
            IList<Employee> employees = await _unitOfWork.GetReadRepository<Employee>().GetAllAsync(
                    enableTracking: false,
                    includeDeleted: true,
                    predicate: e => !e.IsDeleted && !e.IsCurrentlyWorking,
                    orderBy: q => q.OrderByDescending(e => e.InsertedDate));
            if (!employees.Any())
                return (false, "No available employees found.", default!);

            IList<EmployeeDto> employeeDtos = _mapper.Map<IList<EmployeeDto>>(employees);
            return (true, "Available employees retrieved successfully.", employeeDtos);
        }

        /* GetAllOrOnlyActiveEmployeesPagedListAsync method can use for both only active or active and passive lists. */
        public async Task<(bool IsSuccess, string Message, IList<EmployeeDto> Employees, int TotalEmployees)> GetAllOrOnlyActiveEmployeesPagedListAsync(int page = 1, int size = 5, bool includeDeleted = false)
        {
            if (page < 1 || size < 1)
                return (false, "Page and size must be greater than zero.", default!, 0);

            IList<Employee> employees = await _unitOfWork.GetReadRepository<Employee>().GetAllByPagingAsync(
                    enableTracking: false,
                    includeDeleted: includeDeleted,
                    currentPage: page,
                    pageSize: size,
                    orderBy: q => q.OrderByDescending(e => e.InsertedDate));

            int totalEmployees = await _unitOfWork.GetReadRepository<Employee>().CountAsync(includeDeleted: includeDeleted);

            if (!employees.Any())
                return (false, "No employees found.", default!, totalEmployees);

            IList<EmployeeDto> employeeDtos = _mapper.Map<IList<EmployeeDto>>(employees);
            return (true, "Employees retrieved successfully.", employeeDtos, totalEmployees);
        }

        /* GetDeletedEmployeesPagedListAsync method can use for only and only passive list. */
        public async Task<(bool IsSuccess, string Message, IList<EmployeeDto> Employees, int TotalDeletedEmployees)> GetDeletedEmployeesPagedListAsync(int page = 1, int size = 5)
        {
            if (page < 1 || size < 1)
                return (false, "Page and size must be greater than zero.", default!, 0);

            IList<Employee> deletedEmployees = await _unitOfWork.GetReadRepository<Employee>().GetAllByPagingAsync(
                enableTracking: false,
                includeDeleted: true,
                predicate: e => e.IsDeleted,
                currentPage: page,
                pageSize: size,
                orderBy: q => q.OrderByDescending(e => e.InsertedDate));

            int totalDeletedEmployees = await _unitOfWork.GetReadRepository<Employee>().CountAsync(
                    includeDeleted: true,
                    predicate: e => e.IsDeleted);

            if (!deletedEmployees.Any())
                return (false, "No deleted employees found.", default!, totalDeletedEmployees);

            IList<EmployeeDto> employeeDtos = _mapper.Map<IList<EmployeeDto>>(deletedEmployees);
            return (true, "Deleted employees retrieved successfully.", employeeDtos, totalDeletedEmployees);
        }

        public async Task<(bool IsSuccess, string Message)> UpdateAsync(EmployeeUpdateDto dto)
        {
            if (dto is null)
                return (false, "Invalid employee data.");

            Employee? employee = await _unitOfWork.GetReadRepository<Employee>().GetAsync(
                enableTracking: true,
                includeDeleted: true,
                predicate: e => e.Id == dto.Id);
            if (employee is null)
                return (false, "Employee not found.");
            if (employee.IsDeleted)
                return (false, "Cannot update a deleted employee. Please restore it first.");

            List<string> validationErrors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.FirstName))
                validationErrors.Add("First name cannot be empty.");
            if (string.IsNullOrWhiteSpace(dto.LastName))
                validationErrors.Add("Last name cannot be empty.");

            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (dto.DateOfBirth > today.AddYears(-18))
                validationErrors.Add("Employee must be at least 18 years old.");

            if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
                validationErrors.Add("Phone number is required.");
            if (string.IsNullOrWhiteSpace(dto.Address))
                validationErrors.Add("Address is required.");
            if (dto.Salary <= 0)
                validationErrors.Add("Salary must be greater than zero.");

            if (validationErrors.Any())
                return (false, string.Join(" ", validationErrors));

            if (employee.FirstName == dto.FirstName.Trim() &&
                employee.LastName == dto.LastName.Trim() &&
                employee.DateOfBirth == dto.DateOfBirth &&
                employee.PhoneNumber == dto.PhoneNumber.Trim() &&
                employee.Address == dto.Address.Trim() &&
                employee.Salary == dto.Salary)
                return (true, "No changes detected. Update skipped.");

            _mapper.Map(dto, employee);

            await _unitOfWork.GetWriteRepository<Employee>().UpdateAsync(employee);
            await _unitOfWork.SaveAsync();

            return (true, $"Employee '{dto.FirstName} {dto.LastName}' has been successfully updated.");
        }

        public async Task<(bool IsSuccess, string Message)> SafeDeleteEmployeeAsync(Guid employeeId)
        {
            Employee? employee = await _unitOfWork.GetReadRepository<Employee>().GetAsync(
                enableTracking: true,
                includeDeleted: true,
                predicate: e => e.Id == employeeId);
            if (employee is null)
                return (false, "Employee not found.");
            if (employee.IsDeleted)
                return (false, "Employee is already marked as deleted.");
            if (employee.IsCurrentlyWorking)
                return (false, "Cannot delete an employee who is currently working on a task.");

            employee.IsDeleted = true;

            await _unitOfWork.GetWriteRepository<Employee>().UpdateAsync(employee);
            await _unitOfWork.SaveAsync();

            return (true, $"Employee '{employee.FirstName} {employee.LastName}' has been safely deleted.");
        }

        public async Task<(bool IsSuccess, string Message)> RestoreEmployeeAsync(Guid employeeId)
        {
            Employee? employee = await _unitOfWork.GetReadRepository<Employee>().GetAsync(
                enableTracking: true,
                includeDeleted: true,
                predicate: e => e.Id == employeeId && e.IsDeleted);
            if (employee is null)
                return (false, "Employee not found or already active.");

            employee.IsDeleted = false;

            await _unitOfWork.GetWriteRepository<Employee>().UpdateAsync(employee);
            await _unitOfWork.SaveAsync();

            return (true, $"Employee '{employee.FirstName} {employee.LastName}' has been restored.");
        }
    }
}
