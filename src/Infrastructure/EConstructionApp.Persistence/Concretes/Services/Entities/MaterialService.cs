using AutoMapper;
using EConstructionApp.Application.DTOs.Materials;
using EConstructionApp.Application.Interfaces.Services.Entities;
using EConstructionApp.Application.Interfaces.UnitOfWorks;
using EConstructionApp.Application.Validations.Entities.Materials;
using EConstructionApp.Domain.Entities;
using EConstructionApp.Domain.Entities.Cross;
using EConstructionApp.Persistence.Concretes.Services.Entities.Helpers;
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

        public async Task<(bool IsSuccess, string Message)> InsertAsync(MaterialInsertDto? materialInsertDto)
        {
            if (materialInsertDto is null)
                return (false, "Invalid material data. Please ensure all required fields are provided.");

            MaterialInsertDtoValidator validator = new MaterialInsertDtoValidator();
            FluentValidation.Results.ValidationResult validationResult = await validator.ValidateAsync(materialInsertDto);
            if (!validationResult.IsValid)
                return (false, string.Join(" ", validationResult.Errors.Select(e => e.ErrorMessage)));

            Category? category = await _unitOfWork.GetReadRepository<Category>()
                .GetAsync(
                    enableTracking: false,
                    includeDeleted: true,
                    predicate: c => c.Id == materialInsertDto.CategoryId);
            if (category is null)
                return (false, "The selected category could not be found in the system. Please check and try again.");
            if (category.IsDeleted)
                return (false, "The selected category is inactive. Please restore it before proceeding.");

            Material? existingMaterial = await _unitOfWork.GetReadRepository<Material>().GetAsync(
                enableTracking: false,
                includeDeleted: true,
                predicate: m => m.Name.ToLower() == materialInsertDto.Name.Trim().ToLower() &&
                                              m.CategoryId == materialInsertDto.CategoryId);
            if (existingMaterial is not null)
                return existingMaterial.IsDeleted ? (false, "A material with the same name exists in this category but is inactive. Please restore it instead of adding a new one.") : (false, "A material with the same name already exists in this category.");

            Material material = _mapper.Map<Material>(materialInsertDto);

            await _unitOfWork.GetWriteRepository<Material>().AddAsync(material);
            await _unitOfWork.SaveAsync();

            return (true, $"Material '{materialInsertDto.Name.Trim()}' has been successfully inserted under category '{category.Name}'.");
        }

        public async Task<(bool IsSuccess, string Message)> UpdateAsync(MaterialUpdateDto? materialUpdateDto)
        {
            if (materialUpdateDto is null)
                return (false, "Invalid material data. Please ensure all required fields are provided.");

            MaterialUpdateDtoValidator validator = new MaterialUpdateDtoValidator();
            FluentValidation.Results.ValidationResult validationResult = await validator.ValidateAsync(materialUpdateDto);
            if (!validationResult.IsValid)
                return (false, string.Join(" ", validationResult.Errors.Select(e => e.ErrorMessage)));

            Material? material = await _unitOfWork.GetReadRepository<Material>()
                .GetAsync(
                    enableTracking: true,
                    includeDeleted: true,
                    predicate: m => m.Id == materialUpdateDto.Id);
            if (material is null)
                return (false, "The selected material could not be found in the system. Please check and try again.");
            if (material.IsDeleted)
                return (false, "The selected material is inactive. Please restore it before proceeding.");

            Category? category = await _unitOfWork.GetReadRepository<Category>()
                .GetAsync(
                    enableTracking: false,
                    includeDeleted: true,
                    predicate: c => c.Id == materialUpdateDto.CategoryId);
            if (category is null)
                return (false, "The selected category could not be found in the system. Please check and try again.");
            if (category.IsDeleted)
                return (false, "The selected category is inactive. Please restore it before proceeding.");

            if (material.Name.Trim().ToLower() != materialUpdateDto.Name.Trim().ToLower() || material.CategoryId != materialUpdateDto.CategoryId)
            {
                Material? existingMaterial = await _unitOfWork.GetReadRepository<Material>()
                    .GetAsync(
                        enableTracking: false,
                        includeDeleted: true,
                        predicate: m => m.Name.ToLower() == materialUpdateDto.Name.Trim().ToLower() &&
                                        m.CategoryId == materialUpdateDto.CategoryId &&
                                        m.Id != materialUpdateDto.Id);
                if (existingMaterial is not null)
                    return existingMaterial.IsDeleted ? (false, "A material with the same name exists in this category but is inactive. Please restore it instead of updating.") : (false, "A material with the same name already exists in this category.");
            }

            (bool IsSuccess, string Message) checkChanges = ServiceUtils.CheckForNoMaterialChanges(material, materialUpdateDto);
            if (checkChanges.IsSuccess)
                return checkChanges;

            _mapper.Map(materialUpdateDto, material);

            await _unitOfWork.GetWriteRepository<Material>().UpdateAsync(material);
            await _unitOfWork.SaveAsync();

            return (true, $"Material '{materialUpdateDto.Name.Trim()}' has been successfully updated under category '{category.Name}'.");
        }

        public async Task<(bool IsSuccess, string Message)> SafeDeleteAsync(Guid materialId)
        {
            Material? material = await _unitOfWork.GetReadRepository<Material>()
                .GetAsync(
                    enableTracking: true,
                    includeDeleted: true,
                    predicate: m => m.Id == materialId);
            if (material is null)
                return (false, "Material not found in the system. Please ensure the material exists.");
            if (material.IsDeleted)
                return (false, "Material is already marked as deleted.");

            MaterialTask isMaterialAssignedToAnyTask = await _unitOfWork.GetReadRepository<MaterialTask>().GetAsync(
                    enableTracking: false,
                    includeDeleted: false,
                    predicate: mt => mt.MaterialId == materialId);
            if (isMaterialAssignedToAnyTask is not null)
                return (false, $"Material '{material.Name}' cannot be deleted because it is assigned to one or more tasks.");

            material.IsDeleted = true;

            await _unitOfWork.GetWriteRepository<Material>().UpdateAsync(material);
            await _unitOfWork.SaveAsync();

            return (true, $"Material '{material.Name}' has been safely deleted.");
        }

        public async Task<(bool IsSuccess, string Message)> RestoreDeletedAsync(Guid materialId)
        {
            Material? material = await _unitOfWork.GetReadRepository<Material>().GetAsync(
                enableTracking: true,
                includeDeleted: true,
                predicate: m => m.Id == materialId);
            if (material is null)
                return (false, "Material not found in the system. Please ensure the material exists.");
            if (!material.IsDeleted)
                return (false, "Material is already active.");

            bool categoryIsDeleted = await _unitOfWork.GetReadRepository<Category>()
                .GetAsync(
                    enableTracking: false,
                    includeDeleted: true,
                    predicate: c => c.Id == material.CategoryId && c.IsDeleted) is not null;
            if (categoryIsDeleted)
                return (false, $"Material '{material.Name}' cannot be restored because its associated category is deleted. Please restore the category first.");

            material.IsDeleted = false;

            await _unitOfWork.GetWriteRepository<Material>().UpdateAsync(material);
            await _unitOfWork.SaveAsync();

            return (true, $"Material '{material.Name}' has been successfully restored.");
        }

        public async Task<(bool IsSuccess, string Message, int ActiveMaterials, int TotalMaterials)> GetBothActiveAndTotalCountsAsync()
        {
            int totalMaterials = await _unitOfWork.GetReadRepository<Material>().CountAsync(includeDeleted: true);
            if (totalMaterials == 0)
                return (false, "No materials exist in the system.", default!, default!);

            int activeMaterials = await _unitOfWork.GetReadRepository<Material>().CountAsync(includeDeleted: false);

            return (true, $"Currently, {activeMaterials} out of {totalMaterials} materials are active.", activeMaterials, totalMaterials);
        }

        public async Task<(bool IsSuccess, string Message, IList<MaterialDto>? Materials)> GetAvailableMaterialsListAsync()
        {
            IList<Material> materials = await _unitOfWork.GetReadRepository<Material>().GetAllAsync(
                    enableTracking: false,
                    includeDeleted: false,
                    predicate: m => m.StockQuantity > 0,
                    include: entity => entity.Include(m => m.Category),
                    orderBy: q => q.OrderBy(m => m.Category.Name).ThenBy(m => m.Name));
            if (!materials.Any())
                return (false, "No active materials found.", null);

            IList<MaterialDto> materialDtos = _mapper.Map<IList<MaterialDto>>(materials);
            return (true, "Available materials have been successfully retrieved.", materialDtos);
        }

        public async Task<(bool IsSuccess, string Message, IList<MaterialDto>? Materials, int TotalMaterials)> GetOnlyActiveMaterialsPagedListAsync(int pages = 1, int sizes = 5)
        {
            (bool IsValid, string? ErrorMessage) validation = ServiceUtils.ValidatePagination(pages, sizes);
            if (!validation.IsValid)
                return (false, validation.ErrorMessage!, null, default!);

            IList<Material> materials = await _unitOfWork.GetReadRepository<Material>().GetAllByPagingAsync(
                    enableTracking: false,
                    includeDeleted: false,
                    include: entity => entity.Include(m => m.Category),
                    currentPage: pages,
                    pageSize: sizes,
                    orderBy: q => q.OrderByDescending(m => m.InsertedDate));

            int totalMaterials = await _unitOfWork.GetReadRepository<Material>().CountAsync(includeDeleted: false);

            if (!materials.Any())
                return (false, "No active materials found.", null, totalMaterials);

            IList<MaterialDto> materialDtos = _mapper.Map<IList<MaterialDto>>(materials);
            return (true, "Active materials have been successfully retrieved.", materialDtos, totalMaterials);
        }

        public async Task<(bool IsSuccess, string Message, IList<MaterialDto>? Materials, int TotalDeletedMaterials)> GetDeletedMaterialsPagedListAsync(int pages = 1, int sizes = 5)
        {
            (bool IsValid, string? ErrorMessage) validation = ServiceUtils.ValidatePagination(pages, sizes);
            if (!validation.IsValid)
                return (false, validation.ErrorMessage!, null, default);

            IList<Material> deletedMaterials = await _unitOfWork.GetReadRepository<Material>().GetAllByPagingAsync(
                    enableTracking: false,
                    includeDeleted: true,
                    predicate: m => m.IsDeleted,
                    include: entity => entity.Include(m => m.Category),
                    currentPage: pages,
                    pageSize: sizes,
                    orderBy: q => q.OrderByDescending(m => m.InsertedDate));

            int totalDeletedMaterials = await _unitOfWork.GetReadRepository<Material>().CountAsync(
                    includeDeleted: true,
                    predicate: m => m.IsDeleted);

            if (!deletedMaterials.Any())
                return (false, "No deleted materials found.", null, totalDeletedMaterials);

            IList<MaterialDto> materialDtos = _mapper.Map<IList<MaterialDto>>(deletedMaterials);
            return (true, "Deleted materials have been successfully retrieved.", materialDtos, totalDeletedMaterials);
        }
    }
}
