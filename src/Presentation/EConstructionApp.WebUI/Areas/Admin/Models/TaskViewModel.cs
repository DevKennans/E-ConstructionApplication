using EConstructionApp.Application.DTOs.Employees;
using EConstructionApp.Application.DTOs.Materials;
using EConstructionApp.Application.DTOs.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EConstructionApp.WebUI.Areas.Admin.Models
{
    public class TaskViewModel
    {
        public IList<TaskDto> Tasks { get; set; }
        public TaskDetailsUpdateDto TaskDetails { get; set; }
        public IList<EmployeeDto> Employees { get; set; }
        public IList<MaterialDto> Materials { get; set; }
        public List<SelectListItem> Priorities { get; set; } = new();
        public List<SelectListItem> Statuses { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
