using EConstructionApp.Application.DTOs.Categories;
using EConstructionApp.Application.DTOs.Materials;
using EConstructionApp.Application.Interfaces.Services.Entities;
using EConstructionApp.Domain.Enums;
using EConstructionApp.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EConstructionApp.WebUI.Areas.Admin.Controllers
{
    //[Authorize]
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
            var (isSuccess, message, categories) = await _categoryService.GetOnlyActiveCategoriesListAsync();
            if (!isSuccess)
                TempData["ErrorMessageFromMaterial"] = "Failed to load categories.";

            var model = new CreateMaterialViewModel
            {
                Categories = categories ?? new List<CategoryDto>(),
                Material = new MaterialInsertDto(),
                MeasureList = Enum.GetValues(typeof(Measure))
                                  .Cast<Measure>()
                                  .Select(m => new SelectListItem
                                  {
                                      Value = m.ToString(),
                                      Text = m.ToString()
                                  })
                                  .ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddMaterial(CreateMaterialViewModel model)
        {
            if (!ModelState.IsValid)
                TempData["ErrorMessageFromMaterial"] = "Please fill in all required fields correctly.";
            else
            {
                var (isSuccessCategories, messageCategories, categories) = await _categoryService.GetOnlyActiveCategoriesListAsync();

                var (isSuccess, message) = await _materialService.InsertAsync(model.Material);
                model.Categories = categories!;
                if (isSuccess)
                {
                    TempData["SuccessMessageFromMaterial"] = message;

                    return View(model);
                }

                TempData["ErrorMessageFromMaterial"] = message;
            }

            return View(model);
        }

        public async Task<IActionResult> GetMaterial(int page = 1, int size = 5)
        {
            var (isSuccessCategories, messageCategories, categories) = await _categoryService.GetOnlyActiveCategoriesListAsync();

            var (isSuccess, message, materials, totalMaterials) = await _materialService.GetOnlyActiveMaterialsPagedListAsync(page, size);
            if (!isSuccess)
            {
                TempData["ErrorMessageFromMaterial"] = message;

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
                Materials = materials!,
                CurrentPage = page,
                TotalPages = totalPages,
                Categories = categories!,
                MeasureList = Enum.GetValues(typeof(Measure))
                          .Cast<Measure>()
                          .Select(m => new SelectListItem
                          {
                              Value = m.ToString(),
                              Text = m.ToString()
                          })
                          .ToList()
            };

            return View(model);
        }

        public async Task<IActionResult> GetDeletedMaterials(int page = 1, int size = 5)
        {
            var (isSuccess, message, materials, totalMaterials) = await _materialService.GetDeletedMaterialsPagedListAsync(page, size);
            if (!isSuccess)
            {
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
                Materials = materials!,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RestoreDeletedMaterial(Guid materialId)
        {
            var (isSuccess, message) = await _materialService.RestoreDeletedAsync(materialId);
            if (!isSuccess)
                TempData["ErrorMessageFromMaterial"] = message;
            else
                TempData["SuccessMessageFromMaterial"] = message;

            return RedirectToAction("GetDeletedMaterials");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMaterial(Guid materialId)
        {
            var (isSuccess, message) = await _materialService.SafeDeleteAsync(materialId);
            if (!isSuccess)
            {
                TempData["ErrorMessageFromMaterial"] = message;
                return RedirectToAction("GetMaterial");
            }

            TempData["SuccessMessageFromMaterial"] = message;

            return RedirectToAction("GetMaterial");
        }

        [HttpPost]
        public async Task<IActionResult> EditMaterial(MaterialUpdateDto viewModel)
        {
            var (isSuccess, message) = await _materialService.UpdateAsync(viewModel);
            if (!isSuccess)
                TempData["ErrorMessageFromMaterial"] = message;
            else
                TempData["SuccessMessageFromMaterial"] = message;

            return RedirectToAction("GetMaterial");
        }
    }
}