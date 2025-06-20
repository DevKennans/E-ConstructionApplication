﻿using AutoMapper;
using EConstructionApp.Application.DTOs.Employees;
using EConstructionApp.Application.DTOs.Employees.Relations;
using EConstructionApp.Application.Interfaces.Services.Entities;
using EConstructionApp.Application.Interfaces.UnitOfWorks;
using EConstructionApp.Application.Validations.Entities.Employees;
using EConstructionApp.Domain.Entities;
using EConstructionApp.Domain.Entities.Identification;
using EConstructionApp.Persistence.Concretes.Services.Entities.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EConstructionApp.Persistence.Concretes.Services.Entities
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        public EmployeeService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<(bool IsSuccess, string Message)> InsertAsync(EmployeeInsertDto? employeeInsertDto)
        {
            if (employeeInsertDto is null)
                return (false, "Invalid employee data. Please ensure all required fields are provided.");

            EmployeeInsertDtoValidator validator = new EmployeeInsertDtoValidator();
            FluentValidation.Results.ValidationResult validationResult = await validator.ValidateAsync(employeeInsertDto);
            if (!validationResult.IsValid)
                return (false, string.Join(" ", validationResult.Errors.Select(e => e.ErrorMessage)));

            Employee? existingEmployeeWithPhoneNumber = await _unitOfWork.GetReadRepository<Employee>().GetAsync(
                enableTracking: false,
                includeDeleted: true,
                predicate: e => e.PhoneNumber == employeeInsertDto.PhoneNumber);
            if (existingEmployeeWithPhoneNumber is not null)
                return (false, "An employee with the same phone number already exists. Please use a different number.");


            Employee employee = _mapper.Map<Employee>(employeeInsertDto);

            await _unitOfWork.GetWriteRepository<Employee>().AddAsync(employee);
            await _unitOfWork.SaveAsync();

            return (true, $"Employee '{employeeInsertDto.FirstName} {employeeInsertDto.LastName}' has been successfully added.");
        }

        public async Task<(bool IsSuccess, string Message)> UpdateAsync(EmployeeUpdateDto? employeeUpdateDto)
        {
            if (employeeUpdateDto is null)
                return (false, "Invalid employee data. Please ensure all required fields are provided.");

            Employee? employee = await _unitOfWork.GetReadRepository<Employee>().GetAsync(
                enableTracking: true,
                includeDeleted: true,
                predicate: e => e.Id == employeeUpdateDto.Id);
            if (employee is null)
                return (false, "The selected employee could not be found in the system. Please check and try again.");
            if (employee.IsDeleted)
                return (false, "The selected employee is inactive. Please restore it before proceeding.");

            EmployeeUpdateDtoValidator validator = new EmployeeUpdateDtoValidator();
            FluentValidation.Results.ValidationResult validationResult = await validator.ValidateAsync(employeeUpdateDto);
            if (!validationResult.IsValid)
                return (false, string.Join(" ", validationResult.Errors.Select(e => e.ErrorMessage)));

            Employee? existingEmployeeWithPhoneNumber = await _unitOfWork.GetReadRepository<Employee>().GetAsync(
                enableTracking: false,
                includeDeleted: true,
                predicate: e => e.PhoneNumber == employeeUpdateDto.PhoneNumber && e.Id != employeeUpdateDto.Id);
            if (existingEmployeeWithPhoneNumber is not null)
                return (false, "An employee with the same phone number already exists. Please use a different number.");

            (bool IsSuccess, string Message) checkChanges = ServiceUtils.CheckForNoEmployeeChanges(employee, employeeUpdateDto);
            if (checkChanges.IsSuccess)
                return checkChanges;

            _mapper.Map(employeeUpdateDto, employee);

            await _unitOfWork.GetWriteRepository<Employee>().UpdateAsync(employee);
            await _unitOfWork.SaveAsync();

            return (true, $"Employee '{employeeUpdateDto.FirstName} {employeeUpdateDto.LastName}' has been successfully updated.");
        }

        public async Task<(bool IsSuccess, string Message)> SafeDeleteAsync(Guid employeeId)
        {
            Employee? employee = await _unitOfWork.GetReadRepository<Employee>().GetAsync(
                enableTracking: true,
                includeDeleted: true,
                predicate: e => e.Id == employeeId);
            if (employee is null)
                return (false, "Employee not found in the system. Please ensure the employee exists.");
            if (employee.IsDeleted)
                return (false, "Employee is already marked as deleted.");
            if (employee.IsCurrentlyWorking)
                return (false, "Cannot delete an employee who is currently working on a task.");

            employee.IsDeleted = true;

            await _unitOfWork.GetWriteRepository<Employee>().UpdateAsync(employee);
            await _unitOfWork.SaveAsync();

            return (true, $"Employee '{employee.FirstName} {employee.LastName}' has been safely deleted.");
        }

        public async Task<(bool IsSuccess, string Message)> RestoreDeletedAsync(Guid employeeId)
        {
            Employee? employee = await _unitOfWork.GetReadRepository<Employee>().GetAsync(
                enableTracking: true,
                includeDeleted: true,
                predicate: e => e.Id == employeeId);
            if (employee is null)
                return (false, "Employee not found in the system. Please ensure the employee exists.");
            if (!employee.IsDeleted)
                return (false, "Employee is already active.");

            employee.IsDeleted = false;

            await _unitOfWork.GetWriteRepository<Employee>().UpdateAsync(employee);
            await _unitOfWork.SaveAsync();

            return (true, $"Employee '{employee.FirstName} {employee.LastName}' has been successfully restored.");
        }

        public async Task<(bool IsSuccess, string Message, int ActiveEmployees, int TotalEmployees)> GetBothActiveAndTotalCountsAsync()
        {
            int totalEmployees = await _unitOfWork.GetReadRepository<Employee>().CountAsync(includeDeleted: true);
            if (totalEmployees == 0)
                return (false, "No employees found in the system.", default!, default!);

            int activeEmployees = await _unitOfWork.GetReadRepository<Employee>().CountAsync(
                    includeDeleted: false);

            return (true, $"Currently, {activeEmployees} out of {totalEmployees} employees are active.", activeEmployees, totalEmployees);
        }

        public async Task<(bool IsSuccess, string Message)> RecordAttendanceAsync(Guid employeeId, DateTime scanTime)
        {
            AppUser? appUserEmployee = await _userManager.Users
                .FirstOrDefaultAsync(user => user.Id == employeeId.ToString());
            if (appUserEmployee is null)
                return (false, "No employee found with the provided ID.");

            bool hasEmployeePermission = await _userManager.IsInRoleAsync(appUserEmployee, "Employee");
            if (!hasEmployeePermission)
                return (false, "The selected user does not have permission for attendance tracking.");

            Employee? employee = await _unitOfWork.GetReadRepository<Employee>().GetAsync(
                enableTracking: true,
                includeDeleted: true,
                predicate: emp => emp.PhoneNumber == appUserEmployee.PhoneNumber);
            if (employee is null)
                return (false, "No employee found with the provided ID.");
            if (employee.IsDeleted)
                return (false, "The selected employee is no longer active in the system.");

            DateTime localScanTime = scanTime.ToLocalTime();
            DateOnly today = DateOnly.FromDateTime(localScanTime);

            IList<EmployeeAttendance> todayAttendances = await _unitOfWork.GetReadRepository<EmployeeAttendance>().GetAllAsync(
                enableTracking: true,
                includeDeleted: false,
                predicate: ea => ea.EmployeeId == employee.Id && ea.Dairy == today,
                orderBy: q => q.OrderByDescending(ea => ea.InsertedDate));

            EmployeeAttendance? lastAttendance = todayAttendances.FirstOrDefault();
            if (lastAttendance is null || lastAttendance.CheckOutTime is not null)
            {
                EmployeeAttendance newAttendance = new EmployeeAttendance
                {
                    EmployeeId = employee.Id,
                    Dairy = today,
                    CheckInTime = localScanTime
                };

                await _unitOfWork.GetWriteRepository<EmployeeAttendance>().AddAsync(newAttendance);
                await _unitOfWork.SaveAsync();

                return (true, "Check-in successful. You must check out before leaving.");
            }

            lastAttendance.CheckOutTime = localScanTime;
            await _unitOfWork.GetWriteRepository<EmployeeAttendance>().UpdateAsync(lastAttendance);
            await _unitOfWork.SaveAsync();

            return (true, "Check-out successful. Have a great day!");
        }

        public async Task<List<EmployeeAttendanceDto>> GetAttendancesByDateAsync(DateOnly date)
        {
            IList<EmployeeAttendance> attendances = await _unitOfWork.GetReadRepository<EmployeeAttendance>()
                .GetAllAsync(
                    enableTracking: false,
                    includeDeleted: false,
                    predicate: ea => ea.Dairy == date,
                    include: query => query.Include(ea => ea.Employee));

            return _mapper.Map<List<EmployeeAttendanceDto>>(attendances);
        }

        public async Task<(bool IsSuccess, string Message, EmployeeDto? employee)> GetEmployeeByIdAsync(Guid employeeId)
        {
            AppUser? appUserEmployee = await _userManager.Users
                .FirstOrDefaultAsync(user => user.Id == employeeId.ToString());
            if (appUserEmployee is null)
                return (false, "No employee found with the provided ID.", null!);

            bool hasEmployeePermission = await _userManager.IsInRoleAsync(appUserEmployee, "Employee");
            if (!hasEmployeePermission)
                return (false, "The selected user does not have permission to access employee data. (Check user type again, user must have a 'employee' role for doing operation)", null!);

            Employee? employee = await _unitOfWork.GetReadRepository<Employee>().GetAsync(
                enableTracking: false,
                includeDeleted: true,
                predicate: emp => emp.PhoneNumber == appUserEmployee.PhoneNumber);
            if (employee is null)
                return (false, "No employee found with the provided ID.", null);
            if (employee.IsDeleted)
                return (false, "The selected employee is no longer active in the system.", null);

            EmployeeDto employeeDto = _mapper.Map<EmployeeDto>(employee);
            return (true, "Employee has been successfully retrieved.", employeeDto);
        }

        public async Task<(bool IsSuccess, string Message, IList<EmployeeDto>? Employees)> GetAvailableEmployeesListAsync()
        {
            IList<Employee> employees = await _unitOfWork.GetReadRepository<Employee>().GetAllAsync(
                    enableTracking: false,
                    includeDeleted: true,
                    predicate: e => !e.IsDeleted && !e.IsCurrentlyWorking,
                    orderBy: q => q.OrderByDescending(e => e.InsertedDate));
            if (!employees.Any())
                return (false, "No available employees found.", null);

            IList<EmployeeDto> employeeDtos = _mapper.Map<IList<EmployeeDto>>(employees);
            return (true, "Available employees have been successfully retrieved.", employeeDtos);
        }

        public async Task<(bool IsSuccess, string Message, IList<EmployeeDto>? Employees, int TotalEmployees)> GetOnlyActiveEmployeesPagedListAsync(int pages = 1, int sizes = 5)
        {
            (bool IsValid, string? ErrorMessage) validation = ServiceUtils.ValidatePagination(pages, sizes);
            if (!validation.IsValid)
                return (false, validation.ErrorMessage!, null, default!);

            IList<Employee> employees = await _unitOfWork.GetReadRepository<Employee>().GetAllByPagingAsync(
                    enableTracking: false,
                    includeDeleted: false,
                    currentPage: pages,
                    pageSize: sizes,
                    orderBy: q => q.OrderByDescending(e => e.InsertedDate));

            int totalEmployees = await _unitOfWork.GetReadRepository<Employee>().CountAsync(includeDeleted: false);

            if (!employees.Any())
                return (false, "No active employees found.", null, totalEmployees);

            IList<EmployeeDto> employeeDtos = _mapper.Map<IList<EmployeeDto>>(employees);
            return (true, "Active employees have been successfully retrieved.", employeeDtos, totalEmployees);
        }

        public async Task<(bool IsSuccess, string Message, IList<EmployeeDto>? Employees, int TotalDeletedEmployees)> GetDeletedEmployeesPagedListAsync(int pages = 1, int sizes = 5)
        {
            (bool IsValid, string? ErrorMessage) validation = ServiceUtils.ValidatePagination(pages, sizes);
            if (!validation.IsValid)
                return (false, validation.ErrorMessage!, null, default!);

            IList<Employee> deletedEmployees = await _unitOfWork.GetReadRepository<Employee>().GetAllByPagingAsync(
                enableTracking: false,
                includeDeleted: true,
                predicate: e => e.IsDeleted,
                currentPage: pages,
                pageSize: sizes,
                orderBy: q => q.OrderByDescending(e => e.InsertedDate));

            int totalDeletedEmployees = await _unitOfWork.GetReadRepository<Employee>().CountAsync(
                    includeDeleted: true,
                    predicate: e => e.IsDeleted);
            if (!deletedEmployees.Any())
                return (false, "No deleted employees found.", null, totalDeletedEmployees);

            IList<EmployeeDto> employeeDtos = _mapper.Map<IList<EmployeeDto>>(deletedEmployees);
            return (true, "Deleted employees have been successfully retrieved.", employeeDtos, totalDeletedEmployees);
        }
    }
}
