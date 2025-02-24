using EConstructionApp.Application.Interfaces.Services.Entities;
using EConstructionApp.Application.Interfaces.UnitOfWorks;
using EConstructionApp.Domain.Entities;

namespace EConstructionApp.Persistence.Concretes.Services.Entities
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<(bool IsSuccess, string? Message)> InsertAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return (false, "Category name cannot be empty.");

            bool exists = await _unitOfWork
                .GetReadRepository<Category>()
                .GetAsync(c => c.Name.ToLower() == name.ToLower()) is not null;
            if (exists)
                return (false, "A category with the same name already exists.");

            Category category = new Category { Name = name };
            await _unitOfWork.GetWriteRepository<Category>().AddAsync(category);
            await _unitOfWork.SaveAsync();

            return (true, $"Category '{name}' inserted successfully.");
        }

        public async Task<(bool isSuccess, string message, IList<Category> categories)> GetAllCategoriesAsync(bool includeDeleted = false)
        {
            IList<Category> categories = await _unitOfWork.GetReadRepository<Category>()
                .GetAllAsync(
                    includeDeleted: includeDeleted,
                    orderBy: q => q.OrderByDescending(c => c.InsertedDate),
                    enableTracking: false
                );

            if (!categories.Any())
                return (false, "No categories found.", default!);

            return (true, "Categories retrieved successfully.", categories);
        }

        public async Task<(bool isSuccess, string message, IList<Category> categories, int totalCategories)> GetPagedCategoriesAsync(int page = 1, int size = 5, bool includeDeleted = false)
        {
            if (page < 1 || size < 1)
                return (false, "Page and size must be greater than zero.", default!, 0);

            IList<Category> categories = await _unitOfWork.GetReadRepository<Category>()
                .GetAllByPagingAsync(
                    includeDeleted: includeDeleted,
                    orderBy: q => q.OrderByDescending(c => c.InsertedDate),
                    enableTracking: false,
                    currentPage: page,
                    pageSize: size
                );

            int totalCategories = await _unitOfWork.GetReadRepository<Category>().CountAsync();

            if (!categories.Any())
                return (false, "No categories found.", default!, totalCategories);

            return (true, "Categories retrieved successfully.", categories, totalCategories);
        }

        public async Task<(bool isSuccess, string message)> UpdateCategoryAsync(Guid categoryId, string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                return (false, "Category name cannot be empty.");

            newName = newName.Trim();

            Category? categoryToUpdate = await _unitOfWork.GetReadRepository<Category>()
                .GetAsync(c => c.Id == categoryId, enableTracking: true);

            string oldName = categoryToUpdate.Name.Trim();

            if (string.Equals(oldName, newName, StringComparison.OrdinalIgnoreCase))
                return (false, $"Category is already named '{oldName}'. No changes were made.");

            Category? existingCategory = await _unitOfWork.GetReadRepository<Category>()
                .GetAsync(c => c.Name.Trim().ToLower() == newName.ToLower() && c.Id != categoryId, includeDeleted: true);

            if (existingCategory is not null)
            {
                if (existingCategory.IsDeleted)
                    return (false, $"A deleted category with the name '{newName}' already exists. Consider restoring it instead.");

                return (false, $"An active category with the name '{newName}' already exists.");
            }

            categoryToUpdate.Name = newName;
            await _unitOfWork.GetWriteRepository<Category>().UpdateAsync(categoryToUpdate);
            await _unitOfWork.SaveAsync();

            return (true, $"Category '{oldName}' has been successfully updated to '{newName}'.");
        }
    }
}
