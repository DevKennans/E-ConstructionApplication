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
        public async Task<IActionResult> InsertMaterial([FromBody] MaterialInsertDto Dto)
        {
            if (Dto is null)
                return BadRequest(new { error = "Invalid material data." });

            (bool IsSuccess, string? Message) = await _materialService.InsertAsync(Dto);

            if (!IsSuccess)
                return BadRequest(new { error = Message });

            return Ok(new { Message });
        }

        [HttpGet("GetAllOrOnlyActiveMaterialsList")]
        public async Task<IActionResult> GetAllOrOnlyActiveMaterialsList([FromQuery] bool IncludeDeleted = false)
        {
            (bool IsSuccess, string Message, IList<MaterialDto> Materials) =
                await _materialService.GetAllOrOnlyActiveMaterialsListAsync(IncludeDeleted);

            if (!IsSuccess || Materials == default)
                return NotFound(new { error = Message });

            return Ok(new { Message, Materials });
        }

        [HttpGet("GetAllOrOnlyActiveMaterialsPagedList")]
        public async Task<IActionResult> GetAllOrOnlyActiveMaterialsPagedList([FromQuery] int Page, [FromQuery] int Size, [FromQuery] bool IncludeDeleted = false)
        {
            (bool IsSuccess, string Message, IList<MaterialDto> Materials, int TotalMaterials) =
                await _materialService.GetAllOrOnlyActiveMaterialsPagedListAsync(Page, Size, IncludeDeleted);

            if (!IsSuccess || Materials == default)
                return NotFound(new { error = Message, TotalMaterials });

            return Ok(new { Message, TotalMaterials, Materials });
        }

        [HttpGet("GetDeletedMaterialsPagedList")]
        public async Task<IActionResult> GetDeletedMaterialsPagedList([FromQuery] int Page, [FromQuery] int Size)
        {
            (bool IsSuccess, string Message, IList<MaterialDto> Materials, int TotalDeletedMaterials) =
                await _materialService.GetDeletedMaterialsPagedListAsync(Page, Size);

            if (!IsSuccess || Materials == default)
                return NotFound(new { error = Message, TotalDeletedMaterials });

            return Ok(new { Message, TotalDeletedMaterials, Materials });
        }

        [HttpPut("UpdateMaterial")]
        public async Task<IActionResult> UpdateMaterial([FromBody] MaterialUpdateDto Dto)
        {
            (bool IsSuccess, string? Message) = await _materialService.UpdateAsync(Dto);

            if (!IsSuccess)
                return BadRequest(new { error = Message });

            return Ok(new { Message });
        }

        [HttpDelete("SafeDeleteMaterial/{materialId}")]
        public async Task<IActionResult> SafeDeleteMaterial([FromRoute] Guid materialId)
        {
            (bool IsSuccess, string Message) = await _materialService.SafeDeleteMaterialAsync(materialId);

            if (!IsSuccess)
                return BadRequest(new { error = Message });

            return Ok(new { Message });
        }

        [HttpPut("RestoreMaterial/{materialId}")]
        public async Task<IActionResult> RestoreMaterial([FromRoute] Guid materialId)
        {
            (bool IsSuccess, string Message) = await _materialService.RestoreMaterialAsync(materialId);

            if (!IsSuccess)
                return BadRequest(new { error = Message });

            return Ok(new { Message });
        }
    }
}
