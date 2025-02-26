using AutoMapper;
using EConstructionApp.Application.DTOs.Employees;

namespace EConstructionApp.Infrastructure.AutoMapper.Profiles.Employee
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            CreateMap<EmployeeInsertDto, Domain.Entities.Employee>();
        }
    }
}
