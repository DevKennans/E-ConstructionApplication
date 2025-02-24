using AutoMapper;
using EConstructionApp.Application.DTOs.Categories;

namespace EConstructionApp.Infrastructure.AutoMapper.Profiles.Category
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Domain.Entities.Category, CategoryDto>();
        }
    }
}
