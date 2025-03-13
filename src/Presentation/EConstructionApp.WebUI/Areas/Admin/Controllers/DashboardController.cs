using EConstructionApp.Application.Interfaces.Services.Entities;
using EConstructionApp.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EConstructionApp.WebUI.Areas.Admin.Controllers
{
    
    [Area("Admin")]
    [Authorize]
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
            var (categorySuccess, categoryMessage, activeCategories, totalCategories) = await _categoryService.GetBothActiveAndTotalCountsAsync();
            if (!categorySuccess)
                TempData["ErrorMessage"] = categoryMessage;

            var (materialSuccess, materialMessage, activeMaterials, totalMaterials) = await _materialService.GetBothActiveAndTotalCountsAsync();
            if (!materialSuccess)
                TempData["ErrorMessage"] = materialMessage;

            var (employeeSuccess, employeeMessage, activeEmployees, totalEmployees) = await _employeeService.GetBothActiveAndTotalCountsAsync();
            if (!employeeSuccess)
                TempData["ErrorMessage"] = employeeMessage;

            var (taskSuccess, taskMessage, activeTasks, totalTasks) = await _taskService.GetBothActiveAndTotalCountsAsync();

            var (topCategoriesSuccess, topCategoriesMessage, topCategories) = await _categoryService.GetTopUsedCategoriesWithMaterialsCountsAsync(5);

            var (taskStatusSuccess, taskStatusMessage, taskStatusCounts) = await _taskService.GetListOfTasksCountsByStatusAsync();
            if (!taskSuccess)
                TempData["ErrorMessage"] = taskMessage;

            var model = new DashboardViewModel
            {
                ActiveCategories = activeCategories,
                TotalCategories = totalCategories,
                ActiveMaterials = activeMaterials,
                TotalMaterials = totalMaterials,
                ActiveEmployees = activeEmployees,
                TotalEmployees = totalEmployees,
                ActiveTasks = activeTasks,
                TotalTasks = totalTasks,
                TopCategories = topCategories!,
                TaskStatusCounts = taskStatusCounts!
            };

            return View(model);
        }
    }
}
