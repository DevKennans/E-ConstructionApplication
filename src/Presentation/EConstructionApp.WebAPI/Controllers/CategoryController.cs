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

        [HttpGet("GetAllCategories")]
        public async Task<IActionResult> GetAllCategories([FromQuery] bool includeDeleted = false)
        {
            (bool isSuccess, string message, IList<Domain.Entities.Category> categories) = await _categoryService.GetAllCategoriesAsync(includeDeleted);

            if (!isSuccess || categories == default)
                return NotFound(new { error = message });

            return Ok(new { message, categories });
        }

        [HttpGet("GetPagedCategories")]
        public async Task<IActionResult> GetPagedCategories([FromQuery] int page, [FromQuery] int size, [FromQuery] bool includeDeleted = false)
        {
            (bool isSuccess, string message, IList<Domain.Entities.Category> categories, int totalCategories) = await _categoryService.GetPagedCategoriesAsync(page, size, includeDeleted);

            if (!isSuccess || categories == default)
                return NotFound(new { error = message, totalCategories });

            return Ok(new { message, totalCategories, categories });
        }

        [HttpPut("{categoryId}")]
        public async Task<IActionResult> UpdateCategory(Guid categoryId, [FromBody] string newName)
        {
            (bool isSuccess, string message) = await _categoryService.UpdateCategoryAsync(categoryId, newName);

            if (!isSuccess)
                return BadRequest(new { error = message });

            return Ok(new { message });
        }
    }
}
