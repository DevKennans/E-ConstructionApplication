using AutoMapper;
using EConstructionApp.Application.DTOs.Materials;
using EConstructionApp.Application.Interfaces.Services.Entities;
using EConstructionApp.Application.Interfaces.UnitOfWorks;
using EConstructionApp.Domain.Entities;
using EConstructionApp.Domain.Entities.Cross;
using Microsoft.EntityFrameworkCore;

namespace EConstructionApp.Persistence.Concretes.Services.Entities
{
    public class MaterialService : IMaterialService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public MaterialService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<(bool IsSuccess, string Message)> InsertAsync(MaterialInsertDto dto)
        {
            if (dto is null)
                return (false, "Invalid material data.");

            List<string> validationErrors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.Name) || dto.Name.Trim().Length == 0)
                validationErrors.Add("Material name cannot be empty or whitespace.");
            if (dto.Price <= 0)
                validationErrors.Add("Price must be greater than zero.");
            if (dto.StockQuantity < 0)
                validationErrors.Add("Stock quantity cannot be negative.");

            if (validationErrors.Any())
                return (false, string.Join(" ", validationErrors));

            Category? category = await _unitOfWork.GetReadRepository<Category>()
                .GetAsync(
                    predicate: c => c.Id == dto.CategoryId,
                    includeDeleted: true);
            if (category is null)
                return (false, "Category not found.");
            if (category.IsDeleted)
                return (false, "The selected category is inactive. Please restore it first.");

            Material? existingMaterial = await _unitOfWork.GetReadRepository<Material>().GetAsync(
                    enableTracking: false,
                    includeDeleted: true,
                    predicate: m => m.Name.ToLower() == dto.Name.Trim().ToLower() &&
                                              m.CategoryId == dto.CategoryId);
            if (existingMaterial is not null)
                return existingMaterial.IsDeleted
                    ? (false, "A material with the same name exists in this category but is inactive. Please restore it instead of adding a new one.")
                    : (false, "A material with the same name already exists in this category.");

            Material material = _mapper.Map<Material>(dto);

            await _unitOfWork.GetWriteRepository<Material>().AddAsync(material);
            await _unitOfWork.SaveAsync();

            return (true, $"Material '{dto.Name.Trim()}' has been successfully inserted under category '{category.Name}'.");
        }

        /* GetAllOrOnlyActiveMaterialsListAsync method can use for both only active or active and passive lists. */
        public async Task<(bool IsSuccess, string Message, IList<MaterialDto> Materials)> GetAllOrOnlyActiveMaterialsListAsync(bool includeDeleted = false)
        {
            IList<Material> materials = await _unitOfWork.GetReadRepository<Material>().GetAllAsync(
                    enableTracking: false,
                    includeDeleted: includeDeleted,
                    include: entity => entity.Include(m => m.Category),
                    orderBy: q => q.OrderByDescending(m => m.InsertedDate));
            if (!materials.Any())
                return (false, "No materials found.", default!);

            IList<MaterialDto> materialDtos = _mapper.Map<IList<MaterialDto>>(materials);
            return (true, "Materials retrieved successfully.", materialDtos);
        }

        /* GetAllOrOnlyActiveMaterialsPagedListAsync method can use for both only active or active and passive lists. */
        public async Task<(bool IsSuccess, string Message, IList<MaterialDto> Materials, int TotalMaterials)> GetAllOrOnlyActiveMaterialsPagedListAsync(int page = 1, int size = 5, bool includeDeleted = false)
        {
            if (page < 1 || size < 1)
                return (false, "Page and size must be greater than zero.", default!, 0);

            IList<Material> materials = await _unitOfWork.GetReadRepository<Material>().GetAllByPagingAsync(
                    enableTracking: false,
                    includeDeleted: includeDeleted,
                    include: entity => entity.Include(m => m.Category),
                    currentPage: page,
                    pageSize: size,
                    orderBy: q => q.OrderByDescending(m => m.InsertedDate));

            int totalMaterials = await _unitOfWork.GetReadRepository<Material>().CountAsync(includeDeleted: includeDeleted);

            if (!materials.Any())
                return (false, "No materials found.", default!, totalMaterials);

            IList<MaterialDto> materialDtos = _mapper.Map<IList<MaterialDto>>(materials);
            return (true, "Materials retrieved successfully.", materialDtos, totalMaterials);
        }

        /* GetDeletedMaterialsPagedListAsync method can use for only and only passive list. */
        public async Task<(bool IsSuccess, string Message, IList<MaterialDto> Materials, int TotalDeletedMaterials)> GetDeletedMaterialsPagedListAsync(int page = 1, int size = 5)
        {
            if (page < 1 || size < 1)
                return (false, "Page and size must be greater than zero.", default!, 0);

            IList<Material> deletedMaterials = await _unitOfWork.GetReadRepository<Material>().GetAllByPagingAsync(
                enableTracking: false,
                includeDeleted: true,
                predicate: m => m.IsDeleted,
                include: entity => entity.Include(m => m.Category),
                currentPage: page,
                pageSize: size,
                orderBy: q => q.OrderByDescending(m => m.InsertedDate));

            int totalDeletedMaterials = await _unitOfWork.GetReadRepository<Material>().CountAsync(
                    includeDeleted: true,
                    predicate: m => m.IsDeleted);

            if (!deletedMaterials.Any())
                return (false, "No deleted materials found.", default!, totalDeletedMaterials);

            IList<MaterialDto> materialDtos = _mapper.Map<IList<MaterialDto>>(deletedMaterials);
            return (true, "Deleted materials retrieved successfully.", materialDtos, totalDeletedMaterials);
        }

        public async Task<(bool IsSuccess, string Message)> UpdateAsync(MaterialUpdateDto dto)
        {
            if (dto is null)
                return (false, "Invalid material data.");

            List<string> validationErrors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.Name) || dto.Name.Trim().Length == 0)
                validationErrors.Add("Material name cannot be empty or whitespace.");
            if (dto.Price <= 0)
                validationErrors.Add("Price must be greater than zero.");
            if (dto.StockQuantity < 0)
                validationErrors.Add("Stock quantity cannot be negative.");

            if (validationErrors.Any())
                return (false, string.Join(" ", validationErrors));

            Material? material = await _unitOfWork.GetReadRepository<Material>()
                .GetAsync(
                    enableTracking: true,
                    includeDeleted: true,
                    predicate: m => m.Id == dto.Id);
            if (material is null)
                return (false, "Material not found.");
            if (material.IsDeleted)
                return (false, "Cannot update a deleted material. Please restore it first.");

            Category? category = await _unitOfWork.GetReadRepository<Category>()
                .GetAsync(
                    enableTracking: false,
                    includeDeleted: true,
                    predicate: c => c.Id == dto.CategoryId);
            if (category is null)
                return (false, "Category not found.");
            if (category.IsDeleted)
                return (false, "The selected category is inactive. Please restore it first.");

            if (material.Name.Trim().ToLower() != dto.Name.Trim().ToLower() || material.CategoryId != dto.CategoryId)
            {
                Material? existingMaterial = await _unitOfWork.GetReadRepository<Material>()
                    .GetAsync(
                        enableTracking: false,
                        includeDeleted: true,
                        predicate: m => m.Name.ToLower() == dto.Name.Trim().ToLower() &&
                                        m.CategoryId == dto.CategoryId &&
                                        m.Id != dto.Id);
                if (existingMaterial is not null)
                    return existingMaterial.IsDeleted
                        ? (false, "A material with the same name exists in this category but is inactive. Please restore it instead of updating.")
                        : (false, "A material with the same name already exists in this category.");
            }

            if (material.Name == dto.Name.Trim() &&
                material.Price == dto.Price &&
                material.StockQuantity == dto.StockQuantity &&
                material.CategoryId == dto.CategoryId)
                return (true, "No changes detected. Update skipped.");

            _mapper.Map(dto, material);

            await _unitOfWork.GetWriteRepository<Material>().UpdateAsync(material);
            await _unitOfWork.SaveAsync();

            return (true, $"Material '{dto.Name.Trim()}' has been successfully updated.");
        }

        public async Task<(bool IsSuccess, string Message)> SafeDeleteMaterialAsync(Guid materialId)
        {
            Material? material = await _unitOfWork.GetReadRepository<Material>()
                .GetAsync(
                    enableTracking: true,
                    includeDeleted: true,
                    predicate: m => m.Id == materialId);
            if (material is null)
                return (false, "Material not found.");
            if (material.IsDeleted)
                return (false, "Material is already marked as deleted.");

            MaterialTask isMaterialAssignedToAnyTask = await _unitOfWork.GetReadRepository<MaterialTask>().GetAsync(
                    enableTracking: false,
                    predicate: mt => mt.MaterialId == materialId);
            if (isMaterialAssignedToAnyTask is not null)
                return (false, $"Material '{material.Name}' cannot be deleted because it is assigned to one or more tasks.");

            material.IsDeleted = true;

            await _unitOfWork.GetWriteRepository<Material>().UpdateAsync(material);
            await _unitOfWork.SaveAsync();

            return (true, $"Material '{material.Name}' has been safely deleted.");
        }

        public async Task<(bool IsSuccess, string Message)> RestoreMaterialAsync(Guid materialId)
        {
            Material? material = await _unitOfWork.GetReadRepository<Material>().GetAsync(
                enableTracking: true,
                includeDeleted: true,
                predicate: m => m.Id == materialId && m.IsDeleted);
            if (material is null)
                return (false, "Material not found or already active.");

            bool categoryIsDeleted = await _unitOfWork.GetReadRepository<Category>()
                .GetAsync(
                    enableTracking: false,
                    includeDeleted: true,
                    predicate: c => c.Id == material.CategoryId && c.IsDeleted) is not null;
            if (categoryIsDeleted)
                return (false, "Cannot restore material because its associated category is deleted.");

            material.IsDeleted = false;

            await _unitOfWork.GetWriteRepository<Material>().UpdateAsync(material);
            await _unitOfWork.SaveAsync();

            return (true, $"Material '{material.Name}' has been restored.");
        }
    }
}
