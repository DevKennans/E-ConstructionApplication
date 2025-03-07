using EConstructionApp.Application.DTOs.Categories;
using EConstructionApp.Application.DTOs.Categories.Relations;

namespace EConstructionApp.Application.Interfaces.Services.Entities
{
    public interface ICategoryService
    {
        Task<(bool IsSuccess, string Message)> InsertAsync(string name);

        Task<(bool IsSuccess, string Message)> UpdateAsync(Guid categoryId, string newName);

        Task<(bool IsSuccess, string Message)> SafeDeleteAsync(Guid categoryId);

        Task<(bool IsSuccess, string Message)> RestoreDeletedAsync(Guid categoryId);

        Task<(bool IsSuccess, string Message, int ActiveCategories, int TotalCategories)> GetBothActiveAndTotalCountsAsync();

        Task<(bool IsSuccess, string Message, IList<CategoryDto>? Categories)> GetOnlyActiveCategoriesListAsync();

        Task<(bool IsSuccess, string Message, IList<CategoryDto>? Categories, int TotalCategories)> GetOnlyActiveCategoriesPagedListAsync(int pages = 1, int sizes = 5);

        Task<(bool IsSuccess, string Message, IList<CategoryDto>? Categories, int TotalDeletedCategories)> GetDeletedCategoriesPagedListAsync(int pages = 1, int sizes = 5);

        Task<(bool IsSuccess, string Message, IList<CategoryMaterialCountDto>? Categories)> GetTopUsedCategoriesWithMaterialsCountsAsync(int topCounts = 5);
    }
}
