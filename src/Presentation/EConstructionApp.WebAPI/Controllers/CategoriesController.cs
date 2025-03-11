using EConstructionApp.Application.DTOs.Categories;
using EConstructionApp.Application.Interfaces.Services.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EConstructionApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost("InsertCategories")]
        public async Task<IActionResult> InsertCategories([FromBody] string name)
        {
            (bool IsSuccess, string Message) = await _categoryService.InsertAsync(name);
            if (!IsSuccess)
                return BadRequest(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message });
        }

        [HttpPut("UpdateCategories/{categoryId}")]
        public async Task<IActionResult> UpdateCategories(Guid categoryId, [FromBody] string newName)
        {
            (bool IsSuccess, string Message) = await _categoryService.UpdateAsync(categoryId, newName);
            if (!IsSuccess)
                return BadRequest(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message });
        }

        [HttpDelete("SafeDeleteCategories/{categoryId}")]
        public async Task<IActionResult> SafeDeleteCategories(Guid categoryId)
        {
            (bool IsSuccess, string Message) = await _categoryService.SafeDeleteAsync(categoryId);
            if (!IsSuccess)
                return BadRequest(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message });
        }

        [HttpPut("RestoreCategories/{categoryId}")]
        public async Task<IActionResult> RestoreCategories(Guid categoryId)
        {
            (bool IsSuccess, string Message) = await _categoryService.RestoreDeletedAsync(categoryId);
            if (!IsSuccess)
                return BadRequest(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message });
        }

        [HttpGet("GetCategoriesCounts")]
        public async Task<IActionResult> GetCategoriesCounts()
        {
            (bool IsSuccess, string Message, int ActiveCategories, int TotalCategories) = await _categoryService.GetBothActiveAndTotalCountsAsync();
            if (!IsSuccess)
                return NotFound(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message, ActiveCategories, TotalCategories });
        }

        [HttpGet("GetOnlyActiveCategoriesList")]
        public async Task<IActionResult> GetOnlyActiveCategoriesList()
        {
            (bool IsSuccess, string Message, IList<CategoryDto>? Categories) = await _categoryService.GetOnlyActiveCategoriesListAsync();
            if (!IsSuccess)
                return NotFound(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message, Categories });
        }

        [HttpGet("GetOnlyActiveCategoriesPagedList")]
        public async Task<IActionResult> GetOnlyActiveCategoriesPagedList([FromQuery] int pages = 1, [FromQuery] int sizes = 5)
        {
            (bool IsSuccess, string Message, IList<CategoryDto>? Categories, int TotalCategories) = await _categoryService.GetOnlyActiveCategoriesPagedListAsync(pages, sizes);
            if (!IsSuccess)
                return NotFound(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message, TotalCategories, Categories });
        }

        [HttpGet("GetDeletedCategoriesPagedList")]
        public async Task<IActionResult> GetDeletedCategoriesPagedList([FromQuery] int pages = 1, [FromQuery] int sizes = 5)
        {
            (bool IsSuccess, string Message, IList<CategoryDto>? Categories, int TotalDeletedCategories) = await _categoryService.GetDeletedCategoriesPagedListAsync(pages, sizes);
            if (!IsSuccess)
                return NotFound(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message, TotalDeletedCategories, Categories });
        }

        [HttpGet("GetTopUsedCategoriesWithMaterialsCounts")]
        public async Task<IActionResult> GetTopUsedCategoriesWithMaterialsCounts([FromQuery] int counts = 5)
        {
            (bool IsSuccess, string Message, IList<Application.DTOs.Categories.Relations.CategoryMaterialCountDto>? Categories) = await _categoryService.GetTopUsedCategoriesWithMaterialsCountsAsync(counts);
            if (!IsSuccess)
                return NotFound(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message, Categories });
        }
    }
}
