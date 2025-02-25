using EConstructionApp.Application.DTOs.Materials;
using EConstructionApp.Application.Interfaces.Services.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EConstructionApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialController : ControllerBase
    {
        private readonly IMaterialService _materialService;
        public MaterialController(IMaterialService materialService)
        {
            _materialService = materialService;
        }

        [HttpPost("InsertMaterial")]
        public async Task<IActionResult> InsertMaterial([FromBody] MaterialInsertDto dto)
        {
            if (dto is null)
                return BadRequest(new { error = "Invalid material data." });

            (bool isSuccess, string? message) = await _materialService.InsertAsync(dto);

            if (!isSuccess)
                return BadRequest(new { error = message });

            return Ok(new { message });
        }

    }
}
