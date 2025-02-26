using EConstructionApp.Application.DTOs.Categories;
using EConstructionApp.Application.DTOs.Employees;
using EConstructionApp.Application.Interfaces.Services.Entities;
using EConstructionApp.Persistence.Concretes.Services.Entities;
using EConstructionApp.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;

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
            if (ModelState.IsValid)
            {
                var result = await _employeeService.InsertEmployeeAsync(dto);

                if (result.isSuccess)
                {
                    TempData["SuccessMessage"] = result.message;
                    return View();
                }
                else
                {
                    TempData["ErrorMessage"] = result.message;
                    return View();
                }
            }

            return View(dto); 
        }

        public async Task<IActionResult> GetEmployees(int page = 1, int size = 5)
        {
            var (isSuccess, message, employees, totalEmployees) = await _employeeService.GetAllOrOnlyActiveEmployeesPagedListAsync(page, size);

            if (!isSuccess || employees == null)
            {
                ViewBag.Error = message;
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
    }
}
