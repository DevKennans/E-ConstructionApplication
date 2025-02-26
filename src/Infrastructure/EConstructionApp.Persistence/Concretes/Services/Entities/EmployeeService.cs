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

        public async Task<(bool isSuccess, string message)> InsertEmployeeAsync(EmployeeInsertDto dto)
        {
            if (dto is null)
                return (false, "Invalid employee data.");

            List<string> validationErrors = new();

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
            employee.IsCurrentlyWorking = false;

            await _unitOfWork.GetWriteRepository<Employee>().AddAsync(employee);
            await _unitOfWork.SaveAsync();

            return (true, $"Employee '{dto.FirstName} {dto.LastName}' has been successfully added.");
        }

        /* GetAllOrOnlyActiveEmployeesPagedListAsync method can use for both only active or active and passive lists. */
        public async Task<(bool isSuccess, string message, IList<EmployeeDto> employees, int totalEmployees)> GetAllOrOnlyActiveEmployeesPagedListAsync(int page = 1, int size = 5, bool includeDeleted = false)
        {
            if (page < 1 || size < 1)
                return (false, "Page and size must be greater than zero.", default!, 0);

            IList<Employee> employees = await _unitOfWork.GetReadRepository<Employee>().GetAllByPagingAsync(
                    includeDeleted: includeDeleted,
                    orderBy: q => q.OrderByDescending(e => e.InsertedDate),
                    enableTracking: false,
                    currentPage: page,
                    pageSize: size);

            int totalEmployees = await _unitOfWork.GetReadRepository<Employee>().CountAsync(includeDeleted: includeDeleted);

            if (!employees.Any())
                return (false, "No employees found.", default!, totalEmployees);

            IList<EmployeeDto> employeeDtos = _mapper.Map<IList<EmployeeDto>>(employees);
            return (true, "Employees retrieved successfully.", employeeDtos, totalEmployees);
        }
    }
}
