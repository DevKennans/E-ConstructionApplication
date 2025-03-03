using EConstructionApp.Application.DTOs.Employees;
using EConstructionApp.Application.DTOs.Materials;
using EConstructionApp.Application.DTOs.Tasks;

namespace EConstructionApp.WebUI.Areas.Admin.Models
{
    public class TaskViewModel
    {
        public IList<TaskDto> Tasks { get; set; }
        public TaskDetailsUpdateDto TaskDetails { get; set; }
        public IList<EmployeeDto> Employees { get; set; }
        public IList<MaterialDto> Materials { get; set; }
    }
}
