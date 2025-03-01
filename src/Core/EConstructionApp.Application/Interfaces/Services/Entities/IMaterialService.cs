using EConstructionApp.Application.DTOs.Materials;

namespace EConstructionApp.Application.Interfaces.Services.Entities
{
    public interface IMaterialService
    {
        Task<(bool IsSuccess, string Message)> InsertAsync(MaterialInsertDto dto);

        Task<(bool IsSuccess, string Message, IList<MaterialDto> Materials)> GetAvailableMaterialsListAsync();

        Task<(bool IsSuccess, string Message, IList<MaterialDto> Materials, int TotalMaterials)> GetAllOrOnlyActiveMaterialsPagedListAsync(int page = 1, int size = 5, bool includeDeleted = false);

        Task<(bool IsSuccess, string Message, IList<MaterialDto> Materials, int TotalDeletedMaterials)> GetDeletedMaterialsPagedListAsync(int page = 1, int size = 5);

        Task<(bool IsSuccess, string Message)> UpdateAsync(MaterialUpdateDto dto);

        Task<(bool IsSuccess, string Message)> SafeDeleteMaterialAsync(Guid materialId);

        Task<(bool IsSuccess, string Message)> RestoreMaterialAsync(Guid materialId);
    }
}
