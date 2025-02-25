using EConstructionApp.Application.DTOs.Materials;

namespace EConstructionApp.Application.Interfaces.Services.Entities
{
    public interface IMaterialService
    {
        Task<(bool IsSuccess, string? Message)> InsertAsync(MaterialInsertDto dto);
    }
}
