using EConstructionApp.Application.DTOs.Categories;
using EConstructionApp.Application.DTOs.Employees;

namespace EConstructionApp.WebUI.Areas.Admin.Models
{
    public class EmployeeListViewModel
    {
        public IEnumerable<EmployeeDto> Employees { get; set; } = new List<EmployeeDto>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
