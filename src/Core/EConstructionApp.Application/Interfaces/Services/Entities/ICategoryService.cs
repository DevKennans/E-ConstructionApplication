namespace EConstructionApp.Application.Interfaces.Services.Entities
{
    public interface ICategoryService
    {
        Task<(bool IsSuccess, string? Message)> InsertAsync(string name);
    }
}
