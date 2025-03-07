using EConstructionApp.Application.DTOs.Categories;
using EConstructionApp.Application.DTOs.Materials;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EConstructionApp.WebUI.Areas.Admin.Models
{
    public class MaterialListViewModel
    {
        public IEnumerable<MaterialDto> Materials { get; set; } = new List<MaterialDto>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
        public IEnumerable<MaterialUpdateDto> Material { get; set; } = new List<MaterialUpdateDto>();
        public IEnumerable<SelectListItem> MeasureList { get; set; } = new List<SelectListItem>();
    }
}
