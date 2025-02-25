using AutoMapper;
using EConstructionApp.Application.DTOs.Materials;

namespace EConstructionApp.Infrastructure.AutoMapper.Profiles.Material
{
    public class MaterialProfile : Profile
    {
        public MaterialProfile()
        {
            CreateMap<MaterialInsertDto, Domain.Entities.Material>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.InsertedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore());
        }
    }
}
