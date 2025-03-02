using EConstructionApp.Application.DTOs.Employees;
using EConstructionApp.Application.DTOs.Materials;
using EConstructionApp.Application.DTOs.Tasks;
using EConstructionApp.Application.Interfaces.Services.Entities;
using EConstructionApp.Persistence.Concretes.Services.Entities;
using EConstructionApp.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EConstructionApp.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TaskController : Controller
    {
        private readonly ITaskService _taskService;
        private readonly IEmployeeService _employeeService;
        private readonly IMaterialService _materialService;
        public TaskController(ITaskService taskService, IEmployeeService employeeService, IMaterialService materialService)
        {
            _taskService = taskService;
            _employeeService = employeeService;
            _materialService = materialService;
        }
        public async Task<IActionResult> CreateTask()
        {
            var employeeResult = await _employeeService.GetAvailableEmployeesListAsync();
            var materialResult = await _materialService.GetAvailableMaterialsListAsync();

            var model = new TaskCreateViewModel
            {
                Employees = employeeResult.Employees ?? new List<EmployeeDto>(),
                Materials = materialResult.Materials ?? new List<MaterialDto>()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask(TaskCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the errors in the form.";
                return View(model);
            }

            var (isSuccess, message) = await _taskService.InsertAsync(model.Task);
            var employeeResult = await _employeeService.GetAvailableEmployeesListAsync();
            var materialResult = await _materialService.GetAvailableMaterialsListAsync();

            model.Employees = employeeResult.Employees;
            model.Materials = materialResult.Materials;

            if (isSuccess)
            {
                TempData["SuccessMessage"] = message;
                model.Task = new TaskInsertDto();
                return View(model);
            }

            TempData["ErrorMessage"] = message;
            return View(model);
        }

        public async Task<IActionResult> GetTasks()
        {
            var result = await _taskService.GetAllActiveTasksListAsync();
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Message;
                return View(new List<TaskDto>());
            }
            return View(result.Tasks);
        }
    }
}
