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

        [HttpGet("GetAllOrOnlyActiveMaterialsList")]
        public async Task<IActionResult> GetAllOrOnlyActiveMaterialsList([FromQuery] bool includeDeleted = false)
        {
            (bool isSuccess, string message, IList<MaterialDto> materials) =
                await _materialService.GetAllOrOnlyActiveMaterialsListAsync(includeDeleted);

            if (!isSuccess || materials == default)
                return NotFound(new { error = message });

            return Ok(new { message, materials });
        }

        [HttpGet("GetAllOrOnlyActiveMaterialsPagedList")]
        public async Task<IActionResult> GetAllOrOnlyActiveMaterialsPagedList([FromQuery] int page, [FromQuery] int size, [FromQuery] bool includeDeleted = false)
        {
            (bool isSuccess, string message, IList<MaterialDto> materials, int totalMaterials) =
                await _materialService.GetAllOrOnlyActiveMaterialsPagedListAsync(page, size, includeDeleted);

            if (!isSuccess || materials == default)
                return NotFound(new { error = message, totalMaterials });

            return Ok(new { message, totalMaterials, materials });
        }

        [HttpGet("GetDeletedMaterialsPagedList")]
        public async Task<IActionResult> GetDeletedMaterialsPagedList([FromQuery] int page, [FromQuery] int size)
        {
            (bool isSuccess, string message, IList<MaterialDto> materials, int totalDeletedMaterials) =
                await _materialService.GetDeletedMaterialsPagedListAsync(page, size);

            if (!isSuccess || materials == default)
                return NotFound(new { error = message, totalDeletedMaterials });

            return Ok(new { message, totalDeletedMaterials, materials });
        }

        [HttpDelete("SafeDeleteMaterial/{materialId}")]
        public async Task<IActionResult> SafeDeleteMaterial([FromRoute] Guid materialId)
        {
            (bool isSuccess, string message) = await _materialService.SafeDeleteMaterialAsync(materialId);

            if (!isSuccess)
                return BadRequest(new { error = message });

            return Ok(new { message });
        }

        [HttpPut("RestoreMaterial/{materialId}")]
        public async Task<IActionResult> RestoreMaterial([FromRoute] Guid materialId)
        {
            (bool isSuccess, string message) = await _materialService.RestoreMaterialAsync(materialId);

            if (!isSuccess)
                return BadRequest(new { error = message });

            return Ok(new { message });
        }
    }
}
