using EConstructionApp.Application.DTOs.Materials;

namespace EConstructionApp.Application.DTOs.Tasks.Relations
{
    public class TaskMaterialsDto
    {
        public MaterialDto Material { get; set; }
        public decimal Quantity { get; set; }
    }
}
