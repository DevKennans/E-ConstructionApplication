using AutoMapper;
using EConstructionApp.Application.DTOs.Tasks;
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
        }
    }
}
