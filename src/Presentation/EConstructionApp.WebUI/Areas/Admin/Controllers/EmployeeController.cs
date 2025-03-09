using EConstructionApp.Application.DTOs.Categories;
using EConstructionApp.Application.DTOs.Employees;
using EConstructionApp.Application.DTOs.Materials;
using EConstructionApp.Application.Interfaces.Services.Entities;
using EConstructionApp.Domain.Enums;
using EConstructionApp.Domain.Enums.Employees;
using EConstructionApp.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EConstructionApp.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public IActionResult AddEmployee()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEmployee(EmployeeInsertDto dto)
        {
            var (isSuccess, message) = await _employeeService.InsertAsync(dto);
            if (!isSuccess)
            {
                TempData["ErrorMessage"] = message;

                return View();
            }

            TempData["SuccessMessage"] = message;

            return View();
        }

        public async Task<IActionResult> GetEmployees(int page = 1, int size = 5)
        {
            var (isSuccess, message, employees, totalEmployees) = await _employeeService.GetOnlyActiveEmployeesPagedListAsync(page, size);
            if (!isSuccess)
            {
                TempData["ErrorMessage"] = message;

                return View(new EmployeeListViewModel
                {
                    Employees = new List<EmployeeDto>(),
                    CurrentPage = page,
                    TotalPages = 0
                });
            }

            var totalPages = (int)Math.Ceiling((double)totalEmployees / size);
            var model = new EmployeeListViewModel
            {
                Employees = employees!,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(model);
        }

        public async Task<IActionResult> GetDeletedEmployees(int page = 1, int size = 5)
        {
            var (isSuccess, message, employees, totalEmployees) = await _employeeService.GetDeletedEmployeesPagedListAsync(page, size);
            if (!isSuccess || employees == null)
            {
                return View(new EmployeeListViewModel
                {
                    Employees = new List<EmployeeDto>(),
                    CurrentPage = page,
                    TotalPages = 0
                });
            }

            var totalPages = (int)Math.Ceiling((double)totalEmployees / size);
            var model = new EmployeeListViewModel
            {
                Employees = employees,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RestoreDeletedEmployee(Guid employeeId)
        {
            var (isSuccess, message) = await _employeeService.RestoreDeletedAsync(employeeId);
            if (!isSuccess)
                TempData["ErrorMessage"] = message;
            else
                TempData["SuccessMessage"] = message;

            return RedirectToAction("GetDeletedEmployees");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEmployee(Guid employeeId)
        {
            var (isSuccess, message) = await _employeeService.SafeDeleteAsync(employeeId);
            if (!isSuccess)
            {
                TempData["ErrorMessage"] = message;

                return RedirectToAction("GetEmployees");
            }

            TempData["SuccessMessage"] = message;

            return RedirectToAction("GetEmployees");
        }

        [HttpPost]
        public async Task<IActionResult> EditEmployee(EmployeeUpdateDto viewModel)
        {
            var (isSuccess, message) = await _employeeService.UpdateAsync(viewModel);

            if (!isSuccess)
                TempData["ErrorMessage"] = message;
            else
                TempData["SuccessMessage"] = message;

            return RedirectToAction("GetEmployees");
        }
    }
}
