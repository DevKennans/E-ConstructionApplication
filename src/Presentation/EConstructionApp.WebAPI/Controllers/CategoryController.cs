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
            (bool IsSuccess, string Message) = await _categoryService.InsertAsync(name);
            if (!IsSuccess)
                return BadRequest(new { Message });

            return Ok(new { message = $"Category '{name}' inserted successfully." });
        }

        [HttpPut("UpdateCategory/{categoryId}")]
        public async Task<IActionResult> UpdateCategory(Guid categoryId, [FromBody] string newName)
        {
            (bool IsSuccess, string Message) = await _categoryService.UpdateCategoryAsync(categoryId, newName);
            if (!IsSuccess)
                return BadRequest(new { Message });

            return Ok(new { Message });
        }

        [HttpDelete("SafeDeleteCategory/{categoryId}")]
        public async Task<IActionResult> SafeDeleteCategory(Guid categoryId)
        {
            (bool IsSuccess, string Message) = await _categoryService.SafeDeleteCategoryAsync(categoryId);
            if (!IsSuccess)
                return BadRequest(new { Message });

            return Ok(new { Message });
        }

        [HttpPut("RestoreCategory/{categoryId}")]
        public async Task<IActionResult> RestoreCategory(Guid categoryId)
        {
            (bool IsSuccess, string Message) = await _categoryService.RestoreDeletedCategoryAsync(categoryId);
            if (!IsSuccess)
                return BadRequest(new { Message });

            return Ok(new { Message });
        }

        [HttpGet("GetCategoryCounts")]
        public async Task<IActionResult> GetCategoryCounts()
        {
            (bool IsSuccess, string Message, int ActiveCategories, int TotalCategories) = await _categoryService.GetCategoryCountsAsync();
            if (!IsSuccess)
                return NotFound(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message, ActiveCategories, TotalCategories });
        }

        [HttpGet("GetAllOrOnlyActiveCategoriesList")]
        public async Task<IActionResult> GetAllOrOnlyActiveCategoriesList([FromQuery] bool includeDeleted = false)
        {
            (bool IsSuccess, string Message, IList<CategoryDto> Categories) = await _categoryService.GetAllOrOnlyActiveCategoriesListAsync(includeDeleted);
            if (!IsSuccess || Categories == default)
                return NotFound(new { Message });

            return Ok(new { Message, Categories });
        }

        [HttpGet("GetAllOrOnlyActiveCategoriesPagedList")]
        public async Task<IActionResult> GetAllOrOnlyActiveCategoriesPagedList([FromQuery] int page, [FromQuery] int size, [FromQuery] bool includeDeleted = false)
        {
            (bool IsSuccess, string Message, IList<CategoryDto> Categories, int TotalCategories) = await _categoryService.GetAllOrOnlyActiveCategoriesPagedListAsync(page, size, includeDeleted);
            if (!IsSuccess || Categories == default)
                return NotFound(new { Message, TotalCategories });

            return Ok(new { Message, TotalCategories, Categories });
        }

        [HttpGet("GetDeletedCategoriesPagedList")]
        public async Task<IActionResult> GetDeletedCategoriesPagedList([FromQuery] int page, [FromQuery] int size)
        {
            (bool IsSuccess, string Message, IList<CategoryDto> Categories, int TotalDeletedCategories) = await _categoryService.GetDeletedCategoriesPagedListAsync(page, size);
            if (!IsSuccess || Categories == default)
                return NotFound(new { Message, TotalDeletedCategories });

            return Ok(new { Message, TotalDeletedCategories, Categories });
        }

        [HttpGet("GetTopUsedCategoriesWithMaterialCounts")]
        public async Task<IActionResult> GetTopUsedCategoriesWithMaterialCounts([FromQuery] int count = 5)
        {
            (bool IsSuccess, string Message, IList<Application.DTOs.Categories.Relations.CategoryMaterialCountDto> Categories) = await _categoryService.GetTopUsedCategoriesWithMaterialCountsAsync(count);
            if (!IsSuccess)
                return BadRequest(Message);

            return Ok(Categories);
        }
    }
}
