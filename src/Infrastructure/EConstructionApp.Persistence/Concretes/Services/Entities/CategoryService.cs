using AutoMapper;
using EConstructionApp.Application.DTOs.Categories;
using EConstructionApp.Application.DTOs.Categories.Relations;
using EConstructionApp.Application.Interfaces.Services.Entities;
using EConstructionApp.Application.Interfaces.UnitOfWorks;
using EConstructionApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

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

        public async Task<(bool IsSuccess, string Message)> InsertAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return (false, "Category name cannot be empty.");

            Category? existingCategory = await _unitOfWork.GetReadRepository<Category>().GetAsync(
                    enableTracking: false,
                    includeDeleted: true,
                    predicate: c => c.Name.ToLower() == name.Trim().ToLower());
            if (existingCategory is not null)
            {
                if (!existingCategory.IsDeleted)
                    return (false, "A category with the same name already exists and is active.");
                else
                    return (false, "A category with the same name already exists but is inactive. You can reactivate it.");
            }

            Category category = new Category { Name = name };

            await _unitOfWork.GetWriteRepository<Category>().AddAsync(category);
            await _unitOfWork.SaveAsync();

            return (true, $"Category '{name}' inserted successfully.");
        }

        public async Task<(bool IsSuccess, string Message)> UpdateCategoryAsync(Guid categoryId, string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                return (false, "Category name cannot be empty.");

            newName = newName.Trim();

            Category? categoryToUpdate = await _unitOfWork.GetReadRepository<Category>()
                .GetAsync(
                    enableTracking: true,
                    includeDeleted: false,
                    predicate: c => c.Id == categoryId);
            if (categoryToUpdate is null)
                return (false, $"Category with ID: {categoryId} not found or has been soft-deleted. Please reactivate the category before making any updates.");

            string oldName = categoryToUpdate.Name.Trim();
            if (string.Equals(oldName, newName, StringComparison.OrdinalIgnoreCase))
                return (false, $"Category is already named '{oldName}'. No changes were made.");

            Category? existingCategory = await _unitOfWork.GetReadRepository<Category>()
                .GetAsync(
                    enableTracking: false,
                    includeDeleted: true,
                    predicate: c => c.Name.Trim().ToLower() == newName.ToLower() && c.Id != categoryId);
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

        public async Task<(bool IsSuccess, string Message)> SafeDeleteCategoryAsync(Guid categoryId)
        {
            Category? category = await _unitOfWork.GetReadRepository<Category>().GetAsync(
                enableTracking: true,
                includeDeleted: true,
                predicate: c => c.Id == categoryId);
            if (category is null)
                return (false, "Category not found.");
            if (category.IsDeleted)
                return (false, "Category is already marked as deleted.");

            category.IsDeleted = true;
            await _unitOfWork.GetWriteRepository<Category>().UpdateAsync(category);

            IList<Material> materials = await _unitOfWork.GetReadRepository<Material>()
                .GetAllAsync(
                    enableTracking: true,
                    includeDeleted: true,
                    predicate: m => m.CategoryId == categoryId && !m.IsDeleted);

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

        public async Task<(bool IsSuccess, string Message)> RestoreDeletedCategoryAsync(Guid categoryId)
        {
            Category? category = await _unitOfWork.GetReadRepository<Category>()
                .GetAsync(
                    enableTracking: true,
                    includeDeleted: true,
                    predicate: c => c.Id == categoryId && c.IsDeleted);
            if (category is null)
                return (false, "Category not found or already active.");

            category.IsDeleted = false;
            await _unitOfWork.GetWriteRepository<Category>().UpdateAsync(category);

            IList<Material> materials = await _unitOfWork.GetReadRepository<Material>()
                .GetAllAsync(
                    enableTracking: true,
                    includeDeleted: true,
                    predicate: m => m.CategoryId == categoryId && m.IsDeleted);

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

        public async Task<(bool IsSuccess, string Message, int ActiveCategories, int TotalCategories)> GetCategoryCountsAsync()
        {
            int totalCategories = await _unitOfWork.GetReadRepository<Category>().CountAsync(includeDeleted: true);
            if (totalCategories == 0)
                return (false, "No categories found.", default!, default!);

            int activeCategories = await _unitOfWork.GetReadRepository<Category>().CountAsync(
                    includeDeleted: true,
                    predicate: c => !c.IsDeleted);

            return (true, "Category counts retrieved successfully.", activeCategories, totalCategories);
        }

        /* GetAllOrOnlyActiveCategoriesListAsync method can use for both only active or active and passive lists. */
        public async Task<(bool IsSuccess, string Message, IList<CategoryDto> Categories)> GetAllOrOnlyActiveCategoriesListAsync(bool includeDeleted = false)
        {
            IList<Category> categories = await _unitOfWork.GetReadRepository<Category>()
                .GetAllAsync(
                    enableTracking: false,
                    includeDeleted: includeDeleted,
                    orderBy: q => q.OrderByDescending(c => c.InsertedDate));
            if (!categories.Any())
                return (false, "No categories found.", default!);

            IList<CategoryDto> categoryDtos = _mapper.Map<IList<CategoryDto>>(categories);
            return (true, "Categories retrieved successfully.", categoryDtos);
        }

        /* GetAllOrOnlyActiveCategoriesPagedListAsync method can use for both only active or active and passive lists. */
        public async Task<(bool IsSuccess, string Message, IList<CategoryDto> Categories, int TotalCategories)> GetAllOrOnlyActiveCategoriesPagedListAsync(int page = 1, int size = 5, bool includeDeleted = false)
        {
            if (page < 1 || size < 1)
                return (false, "Page and size must be greater than zero.", default!, 0);

            IList<Category> categories = await _unitOfWork.GetReadRepository<Category>()
                .GetAllByPagingAsync(
                    enableTracking: false,
                    includeDeleted: includeDeleted,
                    currentPage: page,
                    pageSize: size,
                    orderBy: q => q.OrderByDescending(c => c.InsertedDate));

            int totalCategories = await _unitOfWork.GetReadRepository<Category>().CountAsync(includeDeleted: includeDeleted);

            if (!categories.Any())
                return (false, "No categories found.", default!, totalCategories);

            IList<CategoryDto> categoryDtos = _mapper.Map<IList<CategoryDto>>(categories);
            return (true, "Categories retrieved successfully.", categoryDtos, totalCategories);
        }

        /* GetDeletedCategoriesPagedListAsync method can use for only and only passive list. */
        public async Task<(bool IsSuccess, string Message, IList<CategoryDto> Categories, int TotalDeletedCategories)> GetDeletedCategoriesPagedListAsync(int page = 1, int size = 5)
        {
            if (page < 1 || size < 1)
                return (false, "Page and size must be greater than zero.", default!, 0);

            IList<Category> deletedCategories = await _unitOfWork.GetReadRepository<Category>()
                .GetAllByPagingAsync(
                    enableTracking: false,
                    includeDeleted: true,
                    predicate: c => c.IsDeleted,
                    currentPage: page,
                    pageSize: size,
                    orderBy: q => q.OrderByDescending(c => c.InsertedDate));

            int totalDeletedCategories = await _unitOfWork.GetReadRepository<Category>().CountAsync(
                    includeDeleted: true,
                    predicate: c => c.IsDeleted);

            if (!deletedCategories.Any())
                return (false, "No deleted categories found.", default!, totalDeletedCategories);

            IList<CategoryDto> categoryDtos = _mapper.Map<IList<CategoryDto>>(deletedCategories);
            return (true, "Deleted categories retrieved successfully.", categoryDtos, totalDeletedCategories);
        }

        public async Task<(bool IsSuccess, string Message, IList<CategoryMaterialCountDto> Categories)> GetTopUsedCategoriesWithMaterialCountsAsync(int topCount = 5)
        {
            if (topCount < 1)
                return (false, "The count must be at least 1.", default!);

            IList<Category> categories = await _unitOfWork.GetReadRepository<Category>()
                .GetAllAsync(
                    enableTracking: false,
                    predicate: c => c.Materials.Any(m => !m.IsDeleted),
                    include: q => q.Include(c => c.Materials),
                    orderBy: q => q.OrderByDescending(c => c.Materials.Count()));
            if (!categories.Any())
                return (false, "No active categories with materials found.", default!);

            List<CategoryMaterialCountDto> categoryMaterialCounts = categories
                .Take(topCount)
                .Select(c => new CategoryMaterialCountDto
                {
                    CategoryName = c.Name,
                    MaterialCounts = c.Materials.Count()
                }).ToList();
            return (true, "Top used categories with material counts retrieved successfully.", categoryMaterialCounts);
        }
    }
}
