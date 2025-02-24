using EConstructionApp.Domain.Entities;

namespace EConstructionApp.Application.Interfaces.Services.Entities
{
    public interface ICategoryService
    {
        Task<(bool IsSuccess, string? Message)> InsertAsync(string name);

        Task<(bool isSuccess, string message, IList<Category> categories, int totalCategories)> GetPagedCategoriesAsync(int page, int size, bool includeDeleted = false);

    }
}
