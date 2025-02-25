using EConstructionApp.Application.DTOs.Categories;
using EConstructionApp.Application.DTOs.Materials;
using EConstructionApp.Application.Interfaces.Services.Entities;
using EConstructionApp.Domain.Enums;
using EConstructionApp.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EConstructionApp.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MaterialController : Controller
    {
        private readonly IMaterialService _materialService;
        private readonly ICategoryService _categoryService;

        public MaterialController(IMaterialService materialService, ICategoryService categoryService)
        {
            _materialService = materialService;
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> AddMaterial()
        {
            var (isSuccess, message, categories) = await _categoryService.GetAllOrOnlyActiveCategoriesListAsync();
            if (!isSuccess)
            {
                TempData["ErrorMessage"] = "Failed to load categories.";
            }
            var model = new CreateMaterialViewModel
            {
                Categories = categories ?? new List<CategoryDto>(),
                Material = new MaterialInsertDto()
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AddMaterial(CreateMaterialViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill in all required fields correctly.";
            }
            else
            {
                var (isSuccess, message) = await _materialService.InsertAsync(model.Material);
                var (isSuccessCategories, messageCategories, categories) = await _categoryService.GetAllOrOnlyActiveCategoriesListAsync();
                model.Categories = categories;
                if (isSuccess)
                {
                    TempData["SuccessMessage"] = message;
                    return View(model);
                }

                TempData["ErrorMessage"] = message;
            }
            return View(model);
        }
        public async Task<IActionResult> GetMaterial()
        {
            return View();
        }


    }
}