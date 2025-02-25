using EConstructionApp.Application.DTOs.Categories;
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
            var (isSuccess, message, categories, totalCategories) = await _categoryService.GetAllOrOnlyActiveCategoriesPagedListAsync(page, size);

            if (!isSuccess || categories == null)
            {
                ViewBag.Error = message;
                return View(new CategoryListViewModel
                {
                    Categories = new List<CategoryDto>(), 
                    CurrentPage = page,
                    TotalPages = 0
                });
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

        public async Task<IActionResult> GetDeletedCategories(int page = 1, int size = 5)
        {
            var (isSuccess, message, categories, totalCategories) = await _categoryService.GetDeletedCategoriesPagedListAsync(page, size);

            if (!isSuccess || categories == null)
            {
                ViewBag.Error = message;
                return View(new CategoryListViewModel
                {
                    Categories = new List<CategoryDto>(),
                    CurrentPage = page,
                    TotalPages = 0
                });
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
        [HttpPost]
        public async Task<IActionResult> RestoreDeletedCategory(Guid categoryId)
        {
            var (isSuccess, message) = await _categoryService.RestoreDeletedCategoryAsync(categoryId);
            if (!isSuccess)
            {
                TempData["ErrorMessage"] = message;
            }
            else
            {
                TempData["SuccessMessage"] = "Category restored successfully.";
            }
            return RedirectToAction("GetDeletedCategories");
        }
        [HttpPost]
        public async Task<IActionResult> EditCategory(CategoryDto viewModel)
        {
            var (isSuccess, message) = await _categoryService.UpdateCategoryAsync(viewModel.Id, viewModel.Name);

            if (!isSuccess)
            {
                TempData["ErrorMessage"] = message; 
            }
            else
            {
                TempData["SuccessMessage"] = message; 
            }
            return RedirectToAction("GetCategories"); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(Guid categoryId)
        {
            var (isSuccess, message) = await _categoryService.SafeDeleteCategoryAsync(categoryId);

            if (!isSuccess)
            {
                TempData["ErrorMessage"] = message;
                return RedirectToAction("GetCategories");
            }
            TempData["SuccessMessage"] = message;
            return RedirectToAction("GetCategories");
        }
    }
}