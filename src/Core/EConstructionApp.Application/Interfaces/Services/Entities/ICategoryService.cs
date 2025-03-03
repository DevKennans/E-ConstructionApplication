using EConstructionApp.Application.DTOs.Categories;
using EConstructionApp.Application.DTOs.Categories.Relations;

namespace EConstructionApp.Application.Interfaces.Services.Entities
{
    public interface ICategoryService
    {
        Task<(bool IsSuccess, string Message)> InsertAsync(string name);

        Task<(bool IsSuccess, string Message)> UpdateCategoryAsync(Guid categoryId, string newName);

        Task<(bool IsSuccess, string Message)> SafeDeleteCategoryAsync(Guid categoryId);

        Task<(bool IsSuccess, string Message)> RestoreDeletedCategoryAsync(Guid categoryId);

        Task<(bool IsSuccess, string Message, int ActiveCategories, int TotalCategories)> GetCategoryCountsAsync();

        Task<(bool IsSuccess, string Message, IList<CategoryDto> Categories)> GetAllOrOnlyActiveCategoriesListAsync(bool includeDeleted = false);

        Task<(bool IsSuccess, string Message, IList<CategoryDto> Categories, int TotalCategories)> GetAllOrOnlyActiveCategoriesPagedListAsync(int page = 1, int size = 5, bool includeDeleted = false);

        Task<(bool IsSuccess, string Message, IList<CategoryDto> Categories, int TotalDeletedCategories)> GetDeletedCategoriesPagedListAsync(int page = 1, int size = 5);

        Task<(bool IsSuccess, string Message, IList<CategoryMaterialCountDto> Categories)> GetTopUsedCategoriesWithMaterialCountsAsync(int topCount = 5);
    }
}
