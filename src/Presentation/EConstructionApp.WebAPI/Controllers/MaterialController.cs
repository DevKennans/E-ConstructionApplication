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

            (bool IsSuccess, string? Message) = await _materialService.InsertAsync(dto);
            if (!IsSuccess)
                return BadRequest(new { Message });

            return Ok(new { Message });
        }

        [HttpPut("UpdateMaterial")]
        public async Task<IActionResult> UpdateMaterial([FromBody] MaterialUpdateDto dto)
        {
            (bool IsSuccess, string Message) = await _materialService.UpdateAsync(dto);
            if (!IsSuccess)
                return BadRequest(new { Message });

            return Ok(new { Message });
        }

        [HttpDelete("SafeDeleteMaterial/{materialId}")]
        public async Task<IActionResult> SafeDeleteMaterial([FromRoute] Guid materialId)
        {
            (bool IsSuccess, string Message) = await _materialService.SafeDeleteMaterialAsync(materialId);
            if (!IsSuccess)
                return BadRequest(new { Message });

            return Ok(new { Message });
        }

        [HttpPut("RestoreMaterial/{materialId}")]
        public async Task<IActionResult> RestoreMaterial([FromRoute] Guid materialId)
        {
            (bool IsSuccess, string Message) = await _materialService.RestoreMaterialAsync(materialId);
            if (!IsSuccess)
                return BadRequest(new { Message });

            return Ok(new { Message });
        }

        [HttpGet("GetMaterialCounts")]
        public async Task<IActionResult> GetMaterialCounts()
        {
            (bool IsSuccess, string Message, int ActiveMaterials, int TotalMaterials) = await _materialService.GetMaterialCountsAsync();
            if (!IsSuccess)
                return NotFound(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message, ActiveMaterials, TotalMaterials });
        }

        [HttpGet("GetAllOrOnlyActiveMaterialsList")]
        public async Task<IActionResult> GetAllOrOnlyActiveMaterialsList()
        {
            (bool IsSuccess, string Message, IList<MaterialDto> Materials) =
                await _materialService.GetAvailableMaterialsListAsync();
            if (!IsSuccess || Materials == default)
                return NotFound(new { Message });

            return Ok(new { Message, Materials });
        }

        [HttpGet("GetAllOrOnlyActiveMaterialsPagedList")]
        public async Task<IActionResult> GetAllOrOnlyActiveMaterialsPagedList([FromQuery] int page, [FromQuery] int size, [FromQuery] bool includeDeleted = false)
        {
            (bool IsSuccess, string Message, IList<MaterialDto> Materials, int TotalMaterials) =
                await _materialService.GetAllOrOnlyActiveMaterialsPagedListAsync(page, size, includeDeleted);
            if (!IsSuccess || Materials == default)
                return NotFound(new { Message, TotalMaterials });

            return Ok(new { Message, TotalMaterials, Materials });
        }

        [HttpGet("GetDeletedMaterialsPagedList")]
        public async Task<IActionResult> GetDeletedMaterialsPagedList([FromQuery] int page, [FromQuery] int size)
        {
            (bool IsSuccess, string Message, IList<MaterialDto> Materials, int TotalDeletedMaterials) =
                await _materialService.GetDeletedMaterialsPagedListAsync(page, size);
            if (!IsSuccess || Materials == default)
                return NotFound(new { Message, TotalDeletedMaterials });

            return Ok(new { Message, TotalDeletedMaterials, Materials });
        }
    }
}
