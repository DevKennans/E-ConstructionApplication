using EConstructionApp.Application.DTOs.Employees;
using EConstructionApp.Application.DTOs.Materials;
using EConstructionApp.Application.DTOs.Tasks;
using EConstructionApp.Application.DTOs.Tasks.Relations;
using EConstructionApp.Application.Interfaces.Services.Entities;
using EConstructionApp.Domain.Enums.Tasks;
using EConstructionApp.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using TaskStatus = EConstructionApp.Domain.Enums.Tasks.TaskStatus;

namespace EConstructionApp.WebUI.Areas.Admin.Controllers
{
    [Authorize]
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
                Materials = materialResult.Materials ?? new List<MaterialDto>(),
                Priorities = Enum.GetValues(typeof(TaskPriority))
                        .Cast<TaskPriority>()
                        .Select(e => new SelectListItem
                        {
                            Value = e.ToString(),
                            Text = e.ToString()
                        }).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask(TaskCreateViewModel model)
        {
            var (isSuccess, message) = await _taskService.InsertAsync(model.Task);
            var employeeResult = await _employeeService.GetAvailableEmployeesListAsync();
            var materialResult = await _materialService.GetAvailableMaterialsListAsync();

            model.Employees = employeeResult.Employees;
            model.Materials = materialResult.Materials;
            model.Priorities = Enum.GetValues(typeof(TaskPriority))
                        .Cast<TaskPriority>()
                        .Select(e => new SelectListItem
                        {
                            Value = e.ToString(),
                            Text = e.ToString()
                        }).ToList();
            if (isSuccess)
            {
                TempData["SuccessMessage"] = message;
                model.Task = new TaskInsertDto();
                return View(model);
            }

            TempData["ErrorMessage"] = message;
            return View(model);
        }

        public async Task<IActionResult> GetTasks(int page = 1, int size = 6)
        {
            var result = await _taskService.GetOnlyActiveTasksPagedListAsync(page, size);
            var res = await _employeeService.GetAvailableEmployeesListAsync();
            var r = await _materialService.GetAvailableMaterialsListAsync();

            var totalPages = (int)Math.Ceiling((double)result.TotalTasks / size);

            var model = new TaskViewModel
            {
                Tasks = result.IsSuccess ? result.Tasks : new List<TaskDto>(),
                Employees = result.IsSuccess ? res.Employees : new List<EmployeeDto>(),
                Materials = result.IsSuccess ? r.Materials : new List<MaterialDto>(),
                Priorities = Enum.GetValues(typeof(TaskPriority))
                                 .Cast<TaskPriority>()
                                 .Select(e => new SelectListItem
                                 {
                                     Value = e.ToString(),
                                     Text = e.ToString()
                                 }).ToList(),
                Statuses = Enum.GetValues(typeof(TaskStatus))
                               .Cast<TaskStatus>()
                               .Select(e => new SelectListItem
                               {
                                   Value = e.ToString(),
                                   Text = e.ToString()
                               }).ToList(),
                CurrentPage = page,
                TotalPages = totalPages
            };

            if (!result.IsSuccess)
                TempData["ErrorMessage"] = result.Message;

            return View(model);
        }




        [HttpPost]
        public async Task<IActionResult> EditTask(TaskDetailsUpdateDto viewModel)
        {
            var (isSuccess, message) = await _taskService.UpdateTasksDetailsAsync(viewModel);
            if (!isSuccess)
            {
                TempData["ErrorMessage"] = message;
            }
            else
            {
                TempData["SuccessMessage"] = message;
            }
            return RedirectToAction("GetTasks");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTaskEmployees(Guid taskId, string? updatedEmployeeIds)
        {
            var employeeIdList = updatedEmployeeIds?.Split(',')
                                           .Select(id => Guid.TryParse(id, out var guid) ? guid : (Guid?)null)
                                           .Where(guid => guid.HasValue)
                                           .Select(guid => guid.Value)
                                           .ToList() ?? new List<Guid>();
            var (isSuccess, message) = await _taskService.UpdateTasksEmployeesAsync(taskId, employeeIdList);
            if (isSuccess)
            {
                TempData["SuccessMessage"] = message;
            }
            else
            {
                TempData["ErrorMessage"] = message;
            }

            return RedirectToAction("GetTasks");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTaskMaterials(Guid taskId, string updatedMaterialIds)
        {
            List<MaterialAssignmentInsertDto> updatedMaterials;
            try
            {
                updatedMaterials = JsonConvert.DeserializeObject<List<MaterialAssignmentInsertDto>>(updatedMaterialIds);
            }
            catch
            {
                TempData["ErrorMessage"] = "Invalid material data!";
                return RedirectToAction("GetTasks");
            }
            var (isSuccess, message) = await _taskService.UpdateTasksMaterialsAsync(taskId, updatedMaterials);

            TempData[isSuccess ? "SuccessMessage" : "ErrorMessage"] = message;
            return RedirectToAction("GetTasks");
        }
    }
}
