using EConstructionApp.Application.DTOs.Categories.Relations;
using EConstructionApp.Application.DTOs.Tasks.Relations;

namespace EConstructionApp.WebUI.Areas.Admin.Models
{
    public class DashboardViewModel
    {
        public int ActiveCategories { get; set; }
        public int TotalCategories { get; set; }
        public int ActiveMaterials { get; set; }
        public int TotalMaterials { get; set; }
        public int ActiveEmployees { get; set; }
        public int TotalEmployees { get; set; }
        public int ActiveTasks { get; set; }
        public int TotalTasks { get; set; }
        public IList<CategoryMaterialCountDto> TopCategories { get; set; }
        public IList<TaskStatusCountsDto> TaskStatusCounts { get; set; }
    }
}
