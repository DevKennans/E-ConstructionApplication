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

            return (true, null);
        }
    }
}
