using EConstructionApp.Application.DTOs.Employees;
using EConstructionApp.Application.DTOs.Materials;
using EConstructionApp.Application.DTOs.Tasks;

namespace EConstructionApp.WebUI.Areas.Admin.Models
{
    public class TaskCreateViewModel
    {
        public TaskInsertDto Task { get; set; }
        public IEnumerable<EmployeeDto>? Employees { get; set; }
        public IEnumerable<MaterialDto>? Materials { get; set; }

        public TaskCreateViewModel()
        {
            Task = new TaskInsertDto();
            Employees = new List<EmployeeDto>();
            Materials = new List<MaterialDto>();
        }
    }
}
