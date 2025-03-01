using AutoMapper;
using EConstructionApp.Application.DTOs.Tasks;
using EConstructionApp.Application.DTOs.Tasks.Relations;
using EConstructionApp.Domain.Entities.Cross;

namespace EConstructionApp.Infrastructure.AutoMapper.Profiles.Task
{
    public class TaskProfile : Profile
    {
        public TaskProfile()
        {
            CreateMap<TaskInsertDto, Domain.Entities.Task>()
                .ForMember(dest => dest.MaterialTasks, opt => opt.MapFrom(src =>
                    src.MaterialAssignments.Select(ma => new MaterialTask
                    {
                        MaterialId = ma.MaterialId,
                        Quantity = ma.Quantity
                    })));

            CreateMap<Domain.Entities.Task, TaskDto>()
                .ForMember(dest => dest.Employees, opt => opt.MapFrom(src => src.Employees))
                .ForMember(dest => dest.MaterialAssignments, opt => opt.MapFrom(src => src.MaterialTasks))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority));

            CreateMap<MaterialTask, TaskMaterialsDto>()
                .ForMember(dest => dest.Material, opt => opt.MapFrom(src => src.Material))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));
        }
    }
}
