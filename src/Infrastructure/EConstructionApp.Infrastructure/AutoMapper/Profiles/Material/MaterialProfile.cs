﻿using AutoMapper;
using EConstructionApp.Application.DTOs.Materials;
using EConstructionApp.Application.DTOs.Materials.Relations;
using EConstructionApp.Domain.Entities.Relations;

namespace EConstructionApp.Infrastructure.AutoMapper.Profiles.Material
{
    public class MaterialProfile : Profile
    {
        public MaterialProfile()
        {
            CreateMap<MaterialInsertDto, Domain.Entities.Material>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.InsertedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

            CreateMap<MaterialUpdateDto, Domain.Entities.Material>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<Domain.Entities.Material, MaterialDto>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category));

            CreateMap<MaterialTransactionLog, MaterialTransactionLogDto>();
        }
    }
}