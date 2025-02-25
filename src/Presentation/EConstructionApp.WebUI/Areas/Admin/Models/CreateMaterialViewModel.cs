using EConstructionApp.Application.DTOs.Materials;
using EConstructionApp.Application.DTOs.Categories;

namespace EConstructionApp.WebUI.Areas.Admin.Models
{
    public class CreateMaterialViewModel
    {
        public MaterialInsertDto Material { get; set; } = new MaterialInsertDto();
        public IEnumerable<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
    }
}
