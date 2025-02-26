using EConstructionApp.Application.DTOs.Categories;
using EConstructionApp.Application.DTOs.Materials;
using EConstructionApp.Application.Interfaces.Services.Entities;
using EConstructionApp.Domain.Enums;
using EConstructionApp.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Drawing;

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
        public async Task<IActionResult> GetMaterial(int page = 1, int size = 5)
        {
            var (isSuccess, message, materials, totalMaterials) = await _materialService.GetAllOrOnlyActiveMaterialsPagedListAsync(page, size);
            var (isSuccessCategories, messageCategories, categories) = await _categoryService.GetAllOrOnlyActiveCategoriesListAsync();
            if (!isSuccess || materials == null)
            {
                ViewBag.Error = message;
                return View(new MaterialListViewModel
                {
                    Materials = new List<MaterialDto>(),
                    CurrentPage = page,
                    TotalPages = 0,
                });
            }

            var totalPages = (int)Math.Ceiling((double)totalMaterials / size);
            var model = new MaterialListViewModel
            {
                Materials = materials,
                CurrentPage = page,
                TotalPages = totalPages,
                Categories = categories
            };

            return View(model);
        }
        public async Task<IActionResult> GetDeletedMaterials(int page = 1, int size = 5)
        {
            var (isSuccess, message, materials, totalMaterials) = await _materialService.GetDeletedMaterialsPagedListAsync(page, size);

            if (!isSuccess || materials == null)
            {
                TempData["ErrorMessage"] = message;
                return View(new MaterialListViewModel
                {
                    Materials = new List<MaterialDto>(),
                    CurrentPage = page,
                    TotalPages = 0
                });
            }
            var totalPages = (int)Math.Ceiling((double)totalMaterials / size);
            var model = new MaterialListViewModel
            {
                Materials = materials,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> RestoreDeletedMaterial(Guid materialId)
        {
            var (isSuccess, message) = await _materialService.RestoreMaterialAsync(materialId);
            if (!isSuccess)
            {
                TempData["ErrorMessage"] = message;
            }
            else
            {
                TempData["SuccessMessage"] = message;
            }
            return RedirectToAction("GetDeletedMaterials");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMaterial(Guid materialId)
        {
            var (isSuccess, message) = await _materialService.SafeDeleteMaterialAsync(materialId);
            if (!isSuccess)
            {
                TempData["ErrorMessage"] = message;
                return RedirectToAction("GetMaterial");
            }
            TempData["SuccessMessage"] = message;
            return RedirectToAction("GetMaterial");
        }

        [HttpPost]
        public async Task<IActionResult> EditMaterial(MaterialUpdateDto viewModel)
        {
            var (isSuccess, message) = await _materialService.UpdateAsync(viewModel);

            if (!isSuccess)
            {
                TempData["ErrorMessage"] = message;
            }
            else
            {
                TempData["SuccessMessage"] = message;
            }
            return RedirectToAction("GetMaterial");
        }
    }
}