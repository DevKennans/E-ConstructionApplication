using EConstructionApp.Application.DTOs.Materials;

namespace EConstructionApp.Application.Interfaces.Services.Entities
{
    public interface IMaterialService
    {
        Task<(bool IsSuccess, string? Message)> InsertAsync(MaterialInsertDto dto);

        Task<(bool isSuccess, string message, IList<MaterialDto> materials)> GetAllOrOnlyActiveMaterialsListAsync(bool includeDeleted = false);

        Task<(bool isSuccess, string message, IList<MaterialDto> materials, int totalMaterials)> GetAllOrOnlyActiveMaterialsPagedListAsync(int page = 1, int size = 5, bool includeDeleted = false);

        Task<(bool isSuccess, string message, IList<MaterialDto> materials, int totalDeletedMaterials)> GetDeletedMaterialsPagedListAsync(int page = 1, int size = 5);

        Task<(bool isSuccess, string message)> SafeDeleteMaterialAsync(Guid materialId);

        Task<(bool isSuccess, string message)> RestoreMaterialAsync(Guid materialId);
    }
}
