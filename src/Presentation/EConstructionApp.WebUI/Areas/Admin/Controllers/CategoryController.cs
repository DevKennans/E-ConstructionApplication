using EConstructionApp.Application.Interfaces.Services.Entities;
using EConstructionApp.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EConstructionApp.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        public IActionResult AddCategory()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddCategory(string name)
        {
            var (isSuccess, message) = await _categoryService.InsertAsync(name);
            if (!isSuccess)
            {
                ViewBag.Message = message;
                return View();
            }
            ViewBag.Success = message;
            return View();
        }

        public async Task<IActionResult> GetCategories(int page = 1, int size = 5)
        {
            var (isSuccess, message, categories, totalCategories) = await _categoryService.GetPagedCategoriesAsync(page, size);

            if (!isSuccess)
            {
                ViewBag.Error = message;
                return View();
            }
            var totalPages = (int)Math.Ceiling((double)totalCategories / size);
            var model = new CategoryListViewModel
            {
                Categories = categories,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(model);
        }
    }
}
