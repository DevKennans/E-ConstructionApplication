using EConstructionApp.Application.DTOs.Categories;

namespace EConstructionApp.Application.Interfaces.Services.Entities
{
    public interface ICategoryService
    {
        Task<(bool IsSuccess, string? Message)> InsertAsync(string name);

        Task<(bool isSuccess, string message, IList<CategoryDto> categories)> GetAllOrOnlyActiveCategoriesListAsync(bool includeDeleted = false);

        Task<(bool isSuccess, string message, IList<CategoryDto> categories, int totalCategories)> GetAllOrOnlyActiveCategoriesPagedListAsync(int page = 1, int size = 5, bool includeDeleted = false);

        Task<(bool isSuccess, string message, IList<CategoryDto> categories, int totalDeletedCategories)> GetDeletedCategoriesPagedListAsync(int page = 1, int size = 5);

        Task<(bool isSuccess, string message)> UpdateCategoryAsync(Guid categoryId, string newName);

        Task<(bool isSuccess, string message)> SafeDeleteCategoryAsync(Guid categoryId);

        Task<(bool isSuccess, string message)> RestoreDeletedCategoryAsync(Guid categoryId);
    }
}
