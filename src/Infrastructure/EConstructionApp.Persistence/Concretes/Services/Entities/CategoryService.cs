using AutoMapper;
using EConstructionApp.Application.DTOs.Categories;
using EConstructionApp.Application.DTOs.Categories.Relations;
using EConstructionApp.Application.Interfaces.Services.Entities;
using EConstructionApp.Application.Interfaces.UnitOfWorks;
using EConstructionApp.Domain.Entities;
using EConstructionApp.Persistence.Concretes.Services.Entities.Helpers;
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
            string validationMessage = ServiceUtils.ValidateCategoryNameForInsert(name);
            if (!string.IsNullOrEmpty(validationMessage))
                return (false, validationMessage);

            name = name.Trim();

            Category? existingCategory = await _unitOfWork.GetReadRepository<Category>().GetAsync(
                    enableTracking: false,
                    includeDeleted: true,
                    predicate: c => c.Name.ToLower() == name.ToLower());
            if (existingCategory is not null)
                return (IsSuccess: false,
                    Message: existingCategory.IsDeleted ? "A category with this name already exists but is inactive. You can reactivate it." : "A category with this name already exists and is active.");

            Category category = new Category { Name = name };

            await _unitOfWork.GetWriteRepository<Category>().AddAsync(category);
            await _unitOfWork.SaveAsync();

            return (true, $"Category '{name}' has been successfully added.");
        }

        public async Task<(bool IsSuccess, string Message)> UpdateAsync(Guid categoryId, string newName)
        {
            string validationMessage = ServiceUtils.ValidateCategoryNameForUpdate(newName);
            if (!string.IsNullOrEmpty(validationMessage))
                return (false, validationMessage);

            newName = newName.Trim();

            Category? categoryToUpdate = await _unitOfWork.GetReadRepository<Category>()
                .GetAsync(
                    enableTracking: true,
                    includeDeleted: false,
                    predicate: c => c.Id == categoryId);
            if (categoryToUpdate is null)
                return (false, $"No active category found with ID: {categoryId}. Please ensure the category exists and is not deleted.");

            string oldName = categoryToUpdate.Name.Trim();
            if (string.Equals(oldName, newName, StringComparison.OrdinalIgnoreCase))
                return (false, $"The category name is already '{oldName}'. No changes were made.");

            Category? existingCategory = await _unitOfWork.GetReadRepository<Category>()
                .GetAsync(
                    enableTracking: false,
                    includeDeleted: true,
                    predicate: c => c.Name.Trim().ToLower() == newName.ToLower() && c.Id != categoryId);
            if (existingCategory is not null)
            {
                return (IsSuccess: false,
                    Message: existingCategory.IsDeleted ? "A category with this name already exists but is inactive. You can reactivate it." : "A category with this name already exists and is active.");
            }

            categoryToUpdate.Name = newName;

            await _unitOfWork.GetWriteRepository<Category>().UpdateAsync(categoryToUpdate);
            await _unitOfWork.SaveAsync();

            return (true, $"Category name successfully updated from '{oldName}' to '{newName}'.");
        }

        public async Task<(bool IsSuccess, string Message)> SafeDeleteAsync(Guid categoryId)
        {
            Category? category = await _unitOfWork.GetReadRepository<Category>().GetAsync(
                enableTracking: true,
                includeDeleted: true,
                predicate: c => c.Id == categoryId,
                include: categoryQuery => categoryQuery.Include(c => c.Materials)
                                                       .ThenInclude(m => m.MaterialTasks));
            if (category is null)
                return (false, "Category not found. Please ensure the category exists.");
            if (category.IsDeleted)
                return (false, "This category has already been deleted.");

            bool anyMaterialAssignedToTask = category.Materials.Any(predicate: material => material.MaterialTasks.Any());
            if (anyMaterialAssignedToTask)
                return (false, "One or more materials in this category are assigned to tasks. Deletion is not allowed.");

            category.IsDeleted = true;
            await _unitOfWork.GetWriteRepository<Category>().UpdateAsync(category);

            IList<Material> materials = await _unitOfWork.GetReadRepository<Material>()
                .GetAllAsync(
                    enableTracking: true,
                    includeDeleted: false,
                    predicate: m => m.CategoryId == categoryId,
                    include: materialsQuery => materialsQuery.Include(m => m.MaterialTasks));

            IList<Material> materialsToDelete = new List<Material>();

            foreach (Material material in materials)
            {
                if (material.MaterialTasks.Any())
                    continue;

                material.IsDeleted = true;
                materialsToDelete.Add(material);
            }

            foreach (Material material in materialsToDelete)
                await _unitOfWork.GetWriteRepository<Material>().UpdateAsync(material);

            await _unitOfWork.SaveAsync();

            return (true, ServiceUtils.GenerateCategorySafeDeleteMessage(category, materialsToDelete.Count));
        }

        public async Task<(bool IsSuccess, string Message)> RestoreDeletedAsync(Guid categoryId)
        {
            Category? category = await _unitOfWork.GetReadRepository<Category>()
                .GetAsync(
                    enableTracking: true,
                    includeDeleted: true,
                    predicate: c => c.Id == categoryId);
            if (category is null)
                return (false, "The specified category was not found. Please check the category ID and try again.");
            if (!category.IsDeleted)
                return (false, "This category is already active.");

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

            return (true, ServiceUtils.GenerateCategoryRestoreMessage(category, materials.Count));
        }

        public async Task<(bool IsSuccess, string Message, int ActiveCategories, int TotalCategories)> GetBothActiveAndTotalCountsAsync()
        {
            int totalCategories = await _unitOfWork.GetReadRepository<Category>().CountAsync(includeDeleted: true);
            if (totalCategories == 0)
                return (false, "No categories exist in the system.", default!, default!);

            int activeCategories = await _unitOfWork.GetReadRepository<Category>().CountAsync(includeDeleted: false);

            return (true, $"Currently, {activeCategories} out of {totalCategories} categories are active.", activeCategories, totalCategories);
        }

        public async Task<(bool IsSuccess, string Message, IList<CategoryDto>? Categories)> GetOnlyActiveCategoriesListAsync()
        {
            IList<Category> categories = await _unitOfWork.GetReadRepository<Category>()
                .GetAllAsync(
                    enableTracking: false,
                    includeDeleted: false,
                    orderBy: q => q.OrderByDescending(c => c.InsertedDate));
            if (!categories.Any())
                return (false, "No active categories found.", null);

            IList<CategoryDto> categoryDtos = _mapper.Map<IList<CategoryDto>>(categories);
            return (true, "Available categories have been successfully retrieved.", categoryDtos);
        }

        public async Task<(bool IsSuccess, string Message, IList<CategoryDto>? Categories, int TotalCategories)> GetOnlyActiveCategoriesPagedListAsync(int pages = 1, int sizes = 5)
        {
            (bool IsValid, string? ErrorMessage) validation = ServiceUtils.ValidatePagination(pages, sizes);
            if (!validation.IsValid)
                return (false, validation.ErrorMessage!, null, default!);

            IList<Category> categories = await _unitOfWork.GetReadRepository<Category>()
                .GetAllByPagingAsync(
                    enableTracking: false,
                    includeDeleted: false,
                    currentPage: pages,
                    pageSize: sizes,
                    orderBy: q => q.OrderByDescending(c => c.InsertedDate));

            int totalCategories = await _unitOfWork.GetReadRepository<Category>().CountAsync(includeDeleted: false);

            if (!categories.Any())
                return (false, "No active categories found.", null, totalCategories);

            IList<CategoryDto> categoryDtos = _mapper.Map<IList<CategoryDto>>(categories);
            return (true, "Active categories have been successfully retrieved.", categoryDtos, totalCategories);
        }

        public async Task<(bool IsSuccess, string Message, IList<CategoryDto>? Categories, int TotalDeletedCategories)> GetDeletedCategoriesPagedListAsync(int pages = 1, int sizes = 5)
        {
            (bool IsValid, string? ErrorMessage) validation = ServiceUtils.ValidatePagination(pages, sizes);
            if (!validation.IsValid)
                return (false, validation.ErrorMessage!, null, default!);

            IList<Category> deletedCategories = await _unitOfWork.GetReadRepository<Category>()
                .GetAllByPagingAsync(
                    enableTracking: false,
                    includeDeleted: true,
                    predicate: c => c.IsDeleted,
                    currentPage: pages,
                    pageSize: sizes,
                    orderBy: q => q.OrderByDescending(c => c.InsertedDate));

            int totalDeletedCategories = await _unitOfWork.GetReadRepository<Category>().CountAsync(
                    includeDeleted: true,
                    predicate: c => c.IsDeleted);

            if (!deletedCategories.Any())
                return (false, "No deleted categories found.", null, totalDeletedCategories);

            IList<CategoryDto> categoryDtos = _mapper.Map<IList<CategoryDto>>(deletedCategories);
            return (true, "Deleted categories have been successfully retrieved.", categoryDtos, totalDeletedCategories);
        }

        public async Task<(bool IsSuccess, string Message, IList<CategoryMaterialCountDto>? Categories)> GetTopUsedCategoriesWithMaterialsCountsAsync(int topCounts = 5)
        {
            if (topCounts < 1)
                return (false, "The requested count must be at least 1.", null);

            IList<Category> categories = await _unitOfWork.GetReadRepository<Category>()
                .GetAllAsync(
                    enableTracking: false,
                    includeDeleted: false,
                    predicate: c => c.Materials.Any(m => !m.IsDeleted),
                    include: q => q.Include(c => c.Materials),
                    orderBy: q => q.OrderByDescending(c => c.Materials.Count()));
            if (!categories.Any())
                return (false, "The requested count must be at least 1.", null);

            List<CategoryMaterialCountDto> categoryMaterialCounts = categories
                .Take(topCounts)
                .Select(c => new CategoryMaterialCountDto
                {
                    CategoryName = c.Name,
                    MaterialCounts = c.Materials.Count()
                }).ToList();
            return (true, "Top active categories with materials' counts have been successfully retrieved.", categoryMaterialCounts);
        }
    }
}
