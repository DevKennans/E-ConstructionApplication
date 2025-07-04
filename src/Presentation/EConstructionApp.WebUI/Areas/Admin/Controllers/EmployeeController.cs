﻿using EConstructionApp.Application.DTOs.Categories;
using EConstructionApp.Application.DTOs.Employees;
using EConstructionApp.Application.DTOs.Materials;
using EConstructionApp.Application.Interfaces.Services.Entities;
using EConstructionApp.Domain.Enums;
using EConstructionApp.Domain.Enums.Employees;
using EConstructionApp.Persistence.Concretes.Services.Entities;
using EConstructionApp.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EConstructionApp.WebUI.Areas.Admin.Controllers
{
    //[Authorize]
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
                TempData["ErrorMessageFromEmployee"] = message;

                return View();
            }

            TempData["SuccessMessageFromEmployee"] = message;

            return View();
        }

        public async Task<IActionResult> GetEmployees(int page = 1, int size = 5)
        {
            var (isSuccess, message, employees, totalEmployees) = await _employeeService.GetOnlyActiveEmployeesPagedListAsync(page, size);
            if (!isSuccess)
            {
                TempData["ErrorMessageFromEmployee"] = message;

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
                TempData["ErrorMessageFromEmployee"] = message;
            else
                TempData["SuccessMessageFromEmployee"] = message;

            return RedirectToAction("GetDeletedEmployees");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEmployee(Guid employeeId)
        {
            var (isSuccess, message) = await _employeeService.SafeDeleteAsync(employeeId);
            if (!isSuccess)
            {
                TempData["ErrorMessageFromEmployee"] = message;

                return RedirectToAction("GetEmployees");
            }

            TempData["SuccessMessageFromEmployee"] = message;

            return RedirectToAction("GetEmployees");
        }

        [HttpPost]
        public async Task<IActionResult> EditEmployee(EmployeeUpdateDto viewModel)
        {
            var (isSuccess, message) = await _employeeService.UpdateAsync(viewModel);

            if (!isSuccess)
                TempData["ErrorMessageFromEmployee"] = message;
            else
                TempData["SuccessMessageFromEmployee"] = message;

            return RedirectToAction("GetEmployees");
        }

        [HttpGet]
        public async Task<IActionResult> EmployeeAttendance(DateOnly? date)
        {
            date ??= DateOnly.FromDateTime(DateTime.Today); 
            var attendances = await _employeeService.GetAttendancesByDateAsync(date.Value);
            ViewBag.SelectedDate = date.Value.ToString("yyyy-MM-dd"); 
            return View(attendances);
        }
    }
}
