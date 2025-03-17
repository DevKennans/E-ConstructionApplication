using AutoMapper;
using EConstructionApp.Application.DTOs.Employees;
using EConstructionApp.Application.DTOs.Employees.Relations;
using EConstructionApp.Domain.Entities;

namespace EConstructionApp.Infrastructure.AutoMapper.Profiles.Employee
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            CreateMap<EmployeeInsertDto, Domain.Entities.Employee>().ReverseMap();

            CreateMap<EmployeeUpdateDto, Domain.Entities.Employee>().ReverseMap();

            CreateMap<EmployeeDto, Domain.Entities.Employee>().ReverseMap();

            CreateMap<EmployeeAttendance, EmployeeAttendanceDto>()
                .ForMember(dest => dest.EmployeeFullName, opt => opt.MapFrom(src => src.Employee.FirstName + " " + src.Employee.LastName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Employee.PhoneNumber))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Employee.Role));
        }
    }
}
