﻿using EConstructionApp.Application.DTOs.Categories;
using EConstructionApp.Application.Interfaces.Services.Entities;
using EConstructionApp.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EConstructionApp.WebUI.Areas.Admin.Controllers
{
    //[Authorize]
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
                TempData["ErrorMessageFromCategory"] = message;
                return View();
            }
            TempData["SuccessMessageFromCategory"] = message;
            return View();
        }

        public async Task<IActionResult> GetCategories(int page = 1, int size = 5)
        {
            var (isSuccess, message, categories, totalCategories) = await _categoryService.GetOnlyActiveCategoriesPagedListAsync(page, size);
            if (!isSuccess)
            {
                TempData["ErrorMessageFromCategory"] = message;

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
                Categories = categories!,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(model);
        }

        public async Task<IActionResult> GetDeletedCategories(int page = 1, int size = 5)
        {
            var (isSuccess, message, categories, totalCategories) = await _categoryService.GetDeletedCategoriesPagedListAsync(page, size);
            if (!isSuccess)
            {
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
                Categories = categories!,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RestoreDeletedCategory(Guid categoryId)
        {
            var (isSuccess, message) = await _categoryService.RestoreDeletedAsync(categoryId);
            if (!isSuccess)
                TempData["ErrorMessageFromCategory"] = message;
            else
                TempData["SuccessMessageFromCategory"] = message;

            return RedirectToAction("GetDeletedCategories");
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory(CategoryDto viewModel)
        {
            var (isSuccess, message) = await _categoryService.UpdateAsync(viewModel.Id, viewModel.Name);
            if (!isSuccess)
                TempData["ErrorMessageFromCategory"] = message;
            else
                TempData["SuccessMessageFromCategory"] = message;

            return RedirectToAction("GetCategories");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(Guid categoryId)
        {
            var (isSuccess, message) = await _categoryService.SafeDeleteAsync(categoryId);
            if (!isSuccess)
            {
                TempData["ErrorMessageFromCategory"] = message;

                return RedirectToAction("GetCategories");
            }

            TempData["SuccessMessageFromCategory"] = message;

            return RedirectToAction("GetCategories");
        }
    }
}