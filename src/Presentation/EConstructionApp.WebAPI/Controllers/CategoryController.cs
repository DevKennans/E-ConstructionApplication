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
                return BadRequest(new { error = Message });

            return Ok(new { message = $"Category '{name}' inserted successfully." });
        }

        [HttpGet("GetAllOrOnlyActiveCategoriesList")]
        public async Task<IActionResult> GetAllOrOnlyActiveCategoriesList([FromQuery] bool includeDeleted = false)
        {
            (bool IsSuccess, string Message, IList<CategoryDto> Categories) = await _categoryService.GetAllOrOnlyActiveCategoriesListAsync(includeDeleted);
            if (!IsSuccess || Categories == default)
                return NotFound(new { error = Message });

            return Ok(new { Message, Categories });
        }

        [HttpGet("GetAllOrOnlyActiveCategoriesPagedList")]
        public async Task<IActionResult> GetAllOrOnlyActiveCategoriesPagedList([FromQuery] int page, [FromQuery] int size, [FromQuery] bool includeDeleted = false)
        {
            (bool IsSuccess, string Message, IList<CategoryDto> Categories, int TotalCategories) = await _categoryService.GetAllOrOnlyActiveCategoriesPagedListAsync(page, size, includeDeleted);
            if (!IsSuccess || Categories == default)
                return NotFound(new { error = Message, TotalCategories });

            return Ok(new { Message, TotalCategories, Categories });
        }

        [HttpGet("GetDeletedCategoriesPagedList")]
        public async Task<IActionResult> GetDeletedCategoriesPagedList([FromQuery] int page, [FromQuery] int size)
        {
            (bool IsSuccess, string Message, IList<CategoryDto> Categories, int TotalDeletedCategories) = await _categoryService.GetDeletedCategoriesPagedListAsync(page, size);
            if (!IsSuccess || Categories == default)
                return NotFound(new { error = Message, TotalDeletedCategories });

            return Ok(new { Message, TotalDeletedCategories, Categories });
        }

        [HttpPut("{categoryId}")]
        public async Task<IActionResult> UpdateCategory(Guid categoryId, [FromBody] string newName)
        {
            (bool IsSuccess, string Message) = await _categoryService.UpdateCategoryAsync(categoryId, newName);
            if (!IsSuccess)
                return BadRequest(new { error = Message });

            return Ok(new { Message });
        }

        [HttpDelete("SafeDeleteCategory/{categoryId}")]
        public async Task<IActionResult> SafeDeleteCategory(Guid categoryId)
        {
            (bool IsSuccess, string Message) = await _categoryService.SafeDeleteCategoryAsync(categoryId);
            if (!IsSuccess)
                return BadRequest(new { error = Message });

            return Ok(new { Message });
        }

        [HttpPut("RestoreCategory/{categoryId}")]
        public async Task<IActionResult> RestoreCategory(Guid categoryId)
        {
            (bool IsSuccess, string Message) = await _categoryService.RestoreDeletedCategoryAsync(categoryId);
            if (!IsSuccess)
                return BadRequest(new { error = Message });

            return Ok(new { Message });
        }
    }
}
