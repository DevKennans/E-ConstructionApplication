using EConstructionApp.Application.DTOs.Materials;
using EConstructionApp.Application.DTOs.Materials.Relations;

namespace EConstructionApp.Application.Interfaces.Services.Entities
{
    public interface IMaterialService
    {
        Task<(bool IsSuccess, string Message)> InsertAsync(MaterialInsertDto? materialInsertDto);

        Task<(bool IsSuccess, string Message)> UpdateAsync(MaterialUpdateDto? materialUpdateDtodto);

        Task<(bool IsSuccess, string Message)> SafeDeleteAsync(Guid materialId);

        Task<(bool IsSuccess, string Message)> RestoreDeletedAsync(Guid materialId);

        Task<(bool IsSuccess, string Message, int ActiveMaterials, int TotalMaterials)>
            GetBothActiveAndTotalCountsAsync();

        Task<(bool IsSuccess, string Message, IList<MaterialDto>? Materials)> GetAvailableMaterialsListAsync();

        Task<(bool IsSuccess, string Message, IList<MaterialDto>? Materials, int TotalMaterials)>
            GetOnlyActiveMaterialsPagedListAsync(int pages = 1, int sizes = 5);

        Task<(bool IsSuccess, string Message, IList<MaterialDto>? Materials, int TotalDeletedMaterials)>
            GetDeletedMaterialsPagedListAsync(int pages = 1, int sizes = 5);

        Task<(bool IsSuccess, string Message, IList<MaterialTransactionLogDto>? Logs)> GetTransactionLogsAsync();
    }
}