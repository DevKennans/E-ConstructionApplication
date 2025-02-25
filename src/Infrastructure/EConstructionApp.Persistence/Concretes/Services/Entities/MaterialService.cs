using AutoMapper;
using EConstructionApp.Application.DTOs.Materials;
using EConstructionApp.Application.Interfaces.Services.Entities;
using EConstructionApp.Application.Interfaces.UnitOfWorks;
using EConstructionApp.Domain.Entities;

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

        public async Task<(bool IsSuccess, string? Message)> InsertAsync(MaterialInsertDto dto)
        {
            if (dto is null)
                return (false, "Invalid material data.");

            List<string> validationErrors = new();

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
                    predicate: m => m.Name.ToLower() == dto.Name.Trim().ToLower() &&
                                              m.CategoryId == dto.CategoryId,
                    includeDeleted: true);
            if (existingMaterial is not null)
                return existingMaterial.IsDeleted
                    ? (false, "A material with the same name exists in this category but is inactive. Please restore it instead of adding a new one.")
                    : (false, "A material with the same name already exists in this category.");

            Material material = _mapper.Map<Material>(dto);

            await _unitOfWork.GetWriteRepository<Material>().AddAsync(material);
            await _unitOfWork.SaveAsync();

            return (true, $"Material '{dto.Name.Trim()}' has been successfully inserted under category '{category.Name}'.");
        }
    }
}
