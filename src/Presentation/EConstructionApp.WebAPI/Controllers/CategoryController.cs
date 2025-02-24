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

        [HttpPost("Insert")]
        public async Task<IActionResult> InsertCategory([FromBody] string name)
        {
            (bool isSuccess, string? message) = await _categoryService.InsertAsync(name);

            if (!isSuccess)
                return BadRequest(new { error = message });

            return Ok(new { message = $"Category '{name}' inserted successfully." });
        }
    }
}
