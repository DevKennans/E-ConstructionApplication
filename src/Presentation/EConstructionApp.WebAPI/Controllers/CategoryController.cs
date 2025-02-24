using EConstructionApp.Application.DTOs.Categories;
using EConstructionApp.Application.Interfaces.Services.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EConstructionApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost("InsertCategory")]
        public async Task<IActionResult> InsertCategory([FromBody] string name)
        {
            (bool isSuccess, string? message) = await _categoryService.InsertAsync(name);

            if (!isSuccess)
                return BadRequest(new { error = message });

            return Ok(new { message = $"Category '{name}' inserted successfully." });
        }

        [HttpGet("GetAllOrOnlyActiveCategoriesList")]
        public async Task<IActionResult> GetAllOrOnlyActiveCategoriesList([FromQuery] bool includeDeleted = false)
        {
            (bool isSuccess, string message, IList<CategoryDto> categories) = await _categoryService.GetAllOrOnlyActiveCategoriesListAsync(includeDeleted);

            if (!isSuccess || categories == default)
                return NotFound(new { error = message });

            return Ok(new { message, categories });
        }

        [HttpGet("GetAllOrOnlyActiveCategoriesPagedList")]
        public async Task<IActionResult> GetAllOrOnlyActiveCategoriesPagedList([FromQuery] int page, [FromQuery] int size, [FromQuery] bool includeDeleted = false)
        {
            (bool isSuccess, string message, IList<CategoryDto> categories, int totalCategories) = await _categoryService.GetAllOrOnlyActiveCategoriesPagedListAsync(page, size, includeDeleted);

            if (!isSuccess || categories == default)
                return NotFound(new { error = message, totalCategories });

            return Ok(new { message, totalCategories, categories });
        }

        [HttpGet("GetDeletedCategoriesPagedList")]
        public async Task<IActionResult> GetDeletedCategoriesPagedList([FromQuery] int page, [FromQuery] int size)
        {
            (bool isSuccess, string message, IList<CategoryDto> categories, int totalDeletedCategories) = await _categoryService.GetDeletedCategoriesPagedListAsync(page, size);

            if (!isSuccess || categories == default)
                return NotFound(new { error = message, totalDeletedCategories });

            return Ok(new { message, totalDeletedCategories, categories });
        }

        [HttpPut("{categoryId}")]
        public async Task<IActionResult> UpdateCategory(Guid categoryId, [FromBody] string newName)
        {
            (bool isSuccess, string message) = await _categoryService.UpdateCategoryAsync(categoryId, newName);

            if (!isSuccess)
                return BadRequest(new { error = message });

            return Ok(new { message });
        }

        [HttpDelete("SafeDeleteCategory/{categoryId}")]
        public async Task<IActionResult> SafeDeleteCategory(Guid categoryId)
        {
            (bool isSuccess, string message) = await _categoryService.SafeDeleteCategoryAsync(categoryId);

            if (!isSuccess)
                return BadRequest(new { error = message });

            return Ok(new { message });
        }

        [HttpPut("RestoreCategory/{categoryId}")]
        public async Task<IActionResult> RestoreCategory(Guid categoryId)
        {
            (bool isSuccess, string message) = await _categoryService.RestoreDeletedCategoryAsync(categoryId);

            if (!isSuccess)
                return BadRequest(new { error = message });

            return Ok(new { message });
        }
    }
}
