using EConstructionApp.Application.Interfaces.Services.Entities;
using EConstructionApp.Persistence.Concretes.Services.Entities;
using EConstructionApp.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;

namespace EConstructionApp.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IMaterialService _materialService;
        private readonly IEmployeeService _employeeService;
        private readonly ITaskService _taskService;

        public DashboardController(ICategoryService categoryService, IMaterialService materialService, IEmployeeService employeeService, ITaskService taskService)
        {
            _categoryService = categoryService;
            _materialService = materialService;
            _employeeService = employeeService;
            _taskService = taskService;
        }
        public async Task<IActionResult> Index()
        {
            var (categorySuccess, categoryMessage, activeCategories, totalCategories) = await _categoryService.GetCategoryCountsAsync();
            var (materialSuccess, materialMessage, activeMaterials, totalMaterials) = await _materialService.GetMaterialCountsAsync();
            var (employeeSuccess, employeeMessage, activeEmployees, totalEmployees) = await _employeeService.GetEmployeeCountsAsync();
            var (taskSuccess, taskMessage, activeTasks, totalTasks) = await _taskService.GetTaskCountsAsync();

            if (!categorySuccess)
            {
                TempData["ErrorMessage"] = categoryMessage;
            }

            if (!materialSuccess)
            {
                TempData["ErrorMessage"] = materialMessage;
            }

            if (!employeeSuccess)
            {
                TempData["ErrorMessage"] = employeeMessage;
            }

            if (!taskSuccess)
            {
                TempData["ErrorMessage"] = taskMessage;
            }

            var model = new DashboardViewModel
            {
                ActiveCategories = activeCategories,
                TotalCategories = totalCategories,
                ActiveMaterials = activeMaterials,
                TotalMaterials = totalMaterials,
                ActiveEmployees = activeEmployees,
                TotalEmployees = totalEmployees,
                ActiveTasks = activeTasks,
                TotalTasks = totalTasks
            };

            return View(model);
        }



    }
}
