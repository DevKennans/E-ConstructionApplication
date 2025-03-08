using AutoMapper;
using EConstructionApp.Application.DTOs.Employees;

namespace EConstructionApp.Infrastructure.AutoMapper.Profiles.Employee
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            CreateMap<EmployeeInsertDto, Domain.Entities.Employee>().ReverseMap();

            CreateMap<EmployeeUpdateDto, Domain.Entities.Employee>().ReverseMap();

            CreateMap<EmployeeDto, Domain.Entities.Employee>().ReverseMap();
        }
    }
}
