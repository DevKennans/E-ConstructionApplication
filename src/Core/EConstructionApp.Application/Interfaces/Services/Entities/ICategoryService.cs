using EConstructionApp.Domain.Entities;

namespace EConstructionApp.Application.Interfaces.Services.Entities
{
    public interface ICategoryService
    {
        Task<(bool IsSuccess, string? Message)> InsertAsync(string name);

        Task<(bool isSuccess, string message, IList<Category> categories)> GetAllCategoriesAsync(bool includeDeleted = false);

        Task<(bool isSuccess, string message, IList<Category> categories, int totalCategories)> GetPagedCategoriesAsync(int page = 1, int size = 5, bool includeDeleted = false);

        Task<(bool isSuccess, string message)> UpdateCategoryAsync(Guid categoryId, string newName);
    }
}
