using AutoMapper;
using EConstructionApp.Application.DTOs.Categories;
using EConstructionApp.Application.Interfaces.Services.Entities;
using EConstructionApp.Application.Interfaces.UnitOfWorks;
using EConstructionApp.Domain.Entities;

namespace EConstructionApp.Persistence.Concretes.Services.Entities
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<(bool IsSuccess, string? Message)> InsertAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return (false, "Category name cannot be empty.");

            bool exists = await _unitOfWork
                .GetReadRepository<Category>()
                .GetAsync(
                    predicate: c => c.Name.ToLower() == name.ToLower(),
                    includeDeleted: true) is not null;
            if (exists)
                return (false, "A category with the same name already exists.");

            Category category = new Category { Name = name };

            await _unitOfWork.GetWriteRepository<Category>().AddAsync(category);
            await _unitOfWork.SaveAsync();

            return (true, $"Category '{name}' inserted successfully.");
        }

        /* GetAllOrOnlyActiveCategoriesListAsync method can use for both only active or active and passive lists. */
        public async Task<(bool isSuccess, string message, IList<CategoryDto> categories)> GetAllOrOnlyActiveCategoriesListAsync(bool includeDeleted = false)
        {
            IList<Category> categories = await _unitOfWork.GetReadRepository<Category>()
                .GetAllAsync(
                    includeDeleted: includeDeleted,
                    orderBy: q => q.OrderByDescending(c => c.InsertedDate),
                    enableTracking: false);
            if (!categories.Any())
                return (false, "No categories found.", default!);

            IList<CategoryDto> categoryDtos = _mapper.Map<IList<CategoryDto>>(categories);
            return (true, "Categories retrieved successfully.", categoryDtos);
        }

        /* GetAllOrOnlyActiveCategoriesPagedListAsync method can use for both only active or active and passive lists. */
        public async Task<(bool isSuccess, string message, IList<CategoryDto> categories, int totalCategories)> GetAllOrOnlyActiveCategoriesPagedListAsync(int page = 1, int size = 5, bool includeDeleted = false)
        {
            if (page < 1 || size < 1)
                return (false, "Page and size must be greater than zero.", default!, 0);

            IList<Category> categories = await _unitOfWork.GetReadRepository<Category>()
                .GetAllByPagingAsync(
                    includeDeleted: includeDeleted,
                    orderBy: q => q.OrderByDescending(c => c.InsertedDate),
                    enableTracking: false,
                    currentPage: page,
                    pageSize: size);

            int totalCategories = await _unitOfWork.GetReadRepository<Category>().CountAsync(includeDeleted: includeDeleted);

            if (!categories.Any())
                return (false, "No categories found.", default!, totalCategories);

            IList<CategoryDto> categoryDtos = _mapper.Map<IList<CategoryDto>>(categories);
            return (true, "Categories retrieved successfully.", categoryDtos, totalCategories);
        }

        /* GetDeletedCategoriesPagedListAsync method can use for only and only passive list. */
        public async Task<(bool isSuccess, string message, IList<CategoryDto> categories, int totalDeletedCategories)> GetDeletedCategoriesPagedListAsync(int page = 1, int size = 5)
        {
            if (page < 1 || size < 1)
                return (false, "Page and size must be greater than zero.", default!, 0);

            IList<Category> deletedCategories = await _unitOfWork.GetReadRepository<Category>()
                .GetAllByPagingAsync(
                    includeDeleted: true,
                    orderBy: q => q.OrderByDescending(c => c.InsertedDate),
                    enableTracking: false,
                    predicate: c => c.IsDeleted,
                    currentPage: page,
                    pageSize: size);

            int totalDeletedCategories = await _unitOfWork.GetReadRepository<Category>().CountAsync(
                predicate: c => c.IsDeleted,
                includeDeleted: true);

            if (!deletedCategories.Any())
                return (false, "No deleted categories found.", default!, totalDeletedCategories);

            IList<CategoryDto> categoryDtos = _mapper.Map<IList<CategoryDto>>(deletedCategories);
            return (true, "Deleted categories retrieved successfully.", categoryDtos, totalDeletedCategories);
        }

        public async Task<(bool isSuccess, string message)> UpdateCategoryAsync(Guid categoryId, string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                return (false, "Category name cannot be empty.");

            newName = newName.Trim();

            Category? categoryToUpdate = await _unitOfWork.GetReadRepository<Category>()
                .GetAsync(
                    predicate: c => c.Id == categoryId,
                    enableTracking: true,
                    includeDeleted: false);
            if (categoryToUpdate is null)
                return (false, $"Category with ID: {categoryId} not found or has been soft-deleted. Please reactivate the category before making any updates.");

            string oldName = categoryToUpdate.Name.Trim();
            if (string.Equals(oldName, newName, StringComparison.OrdinalIgnoreCase))
                return (false, $"Category is already named '{oldName}'. No changes were made.");

            Category? existingCategory = await _unitOfWork.GetReadRepository<Category>()
                .GetAsync(
                predicate: c => c.Name.Trim().ToLower() == newName.ToLower() && c.Id != categoryId,
                includeDeleted: true);
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

        public async Task<(bool isSuccess, string message)> SafeDeleteCategoryAsync(Guid categoryId)
        {
            Category? category = await _unitOfWork.GetReadRepository<Category>().GetAsync(
                predicate: c => c.Id == categoryId,
                enableTracking: true,
                includeDeleted: true);
            if (category is null)
                return (false, "Category not found.");
            if (category.IsDeleted)
                return (false, "Category is already marked as deleted.");

            category.IsDeleted = true;
            await _unitOfWork.GetWriteRepository<Category>().UpdateAsync(category);

            IList<Material> materials = await _unitOfWork.GetReadRepository<Material>()
                .GetAllAsync(
                    predicate: m => m.CategoryId == categoryId,
                    enableTracking: true);

            foreach (Material material in materials)
            {
                material.IsDeleted = true;
                await _unitOfWork.GetWriteRepository<Material>().UpdateAsync(material);
            }

            await _unitOfWork.SaveAsync();

            string materialMessage = materials.Count switch
            {
                0 => $"Category '{category.Name}' has been safely deleted.",
                1 => $"Category '{category.Name}' and 1 associated material have been safely deleted.",
                _ => $"Category '{category.Name}' and {materials.Count} associated materials have been safely deleted."
            };
            return (true, materialMessage);
        }

        public async Task<(bool isSuccess, string message)> RestoreDeletedCategoryAsync(Guid categoryId)
        {
            Category? category = await _unitOfWork.GetReadRepository<Category>()
                .GetAsync(
                    predicate: c => c.Id == categoryId && c.IsDeleted,
                    enableTracking: true,
                    includeDeleted: true);
            if (category is null)
                return (false, "Category not found or already active.");

            category.IsDeleted = false;
            await _unitOfWork.GetWriteRepository<Category>().UpdateAsync(category);

            IList<Material> materials = await _unitOfWork.GetReadRepository<Material>()
                .GetAllAsync(
                    predicate: m => m.CategoryId == categoryId && m.IsDeleted,
                    enableTracking: true,
                    includeDeleted: true);

            foreach (Material material in materials)
            {
                material.IsDeleted = false;
                await _unitOfWork.GetWriteRepository<Material>().UpdateAsync(material);
            }

            await _unitOfWork.SaveAsync();

            string materialMessage = materials.Count switch
            {
                0 => $"Category '{category.Name}' has been restored.",
                1 => $"Category '{category.Name}' and 1 associated material have been restored.",
                _ => $"Category '{category.Name}' and {materials.Count} associated materials have been restored."
            };
            return (true, materialMessage);
        }
    }
}
