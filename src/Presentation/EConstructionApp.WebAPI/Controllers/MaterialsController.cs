using EConstructionApp.Application.DTOs.Materials;
using EConstructionApp.Application.Interfaces.Services.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EConstructionApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MaterialsController : ControllerBase
    {
        private readonly IMaterialService _materialService;
        public MaterialsController(IMaterialService materialService)
        {
            _materialService = materialService;
        }

        [HttpPost("InsertMaterials")]
        public async Task<IActionResult> InsertMaterials([FromBody] MaterialInsertDto? materialInsertDto)
        {
            (bool IsSuccess, string Message) = await _materialService.InsertAsync(materialInsertDto);
            if (!IsSuccess)
                return BadRequest(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message });
        }

        [HttpPut("UpdateMaterials")]
        public async Task<IActionResult> UpdateMaterials([FromBody] MaterialUpdateDto? materialUpdateDto)
        {
            (bool IsSuccess, string Message) = await _materialService.UpdateAsync(materialUpdateDto);
            if (!IsSuccess)
                return BadRequest(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message });
        }

        [HttpDelete("SafeDeleteMaterials/{materialId}")]
        public async Task<IActionResult> SafeDeleteMaterials([FromRoute] Guid materialId)
        {
            (bool IsSuccess, string Message) = await _materialService.SafeDeleteAsync(materialId);
            if (!IsSuccess)
                return BadRequest(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message });
        }

        [HttpPut("RestoreMaterials/{materialId}")]
        public async Task<IActionResult> RestoreMaterials([FromRoute] Guid materialId)
        {
            (bool IsSuccess, string Message) = await _materialService.RestoreDeletedAsync(materialId);
            if (!IsSuccess)
                return BadRequest(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message });
        }

        [HttpGet("GetMaterialsCounts")]
        public async Task<IActionResult> GetMaterialsCounts()
        {
            (bool IsSuccess, string Message, int ActiveMaterials, int TotalMaterials) = await _materialService.GetBothActiveAndTotalCountsAsync();
            if (!IsSuccess)
                return NotFound(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message, ActiveMaterials, TotalMaterials });
        }

        [HttpGet("GetAvailableMaterialsList")]
        public async Task<IActionResult> GetAvailableMaterialsList()
        {
            (bool IsSuccess, string Message, IList<MaterialDto>? Materials) = await _materialService.GetAvailableMaterialsListAsync();
            if (!IsSuccess)
                return NotFound(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message, Materials });
        }

        [HttpGet("GetOnlyActiveMaterialsPagedList")]
        public async Task<IActionResult> GetOnlyActiveMaterialsPagedList([FromQuery] int pages = 1, [FromQuery] int sizes = 5)
        {
            (bool IsSuccess, string Message, IList<MaterialDto>? Materials, int TotalMaterials) = await _materialService.GetOnlyActiveMaterialsPagedListAsync(pages, sizes);
            if (!IsSuccess)
                return NotFound(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message, TotalMaterials, Materials });
        }

        [HttpGet("GetDeletedMaterialsPagedList")]
        public async Task<IActionResult> GetDeletedMaterialsPagedList([FromQuery] int pages = 1, [FromQuery] int sizes = 5)
        {
            (bool IsSuccess, string Message, IList<MaterialDto>? Materials, int TotalDeletedMaterials) = await _materialService.GetDeletedMaterialsPagedListAsync(pages, sizes);
            if (!IsSuccess)
                return NotFound(new { IsSuccess, Message });

            return Ok(new { IsSuccess, Message, TotalDeletedMaterials, Materials });
        }
    }
}
