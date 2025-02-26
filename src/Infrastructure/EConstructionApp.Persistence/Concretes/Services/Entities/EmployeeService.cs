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

            await _unitOfWork.GetWriteRepository<Employee>().AddAsync(employee);
            await _unitOfWork.SaveAsync();

            return (true, $"Employee '{dto.FirstName} {dto.LastName}' has been successfully added.");
        }
    }
}
