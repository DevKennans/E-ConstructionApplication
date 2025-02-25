using EConstructionApp.Application.DTOs.Categories;
using EConstructionApp.Domain.Entities;

namespace EConstructionApp.WebUI.Areas.Admin.Models
{
    public class CategoryListViewModel
    {
        public IEnumerable<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

    }
}

