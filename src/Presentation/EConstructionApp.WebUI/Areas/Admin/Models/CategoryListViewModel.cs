using EConstructionApp.Domain.Entities;

namespace EConstructionApp.WebUI.Areas.Admin.Models
{
    public class CategoryListViewModel
    {
        public IEnumerable<Category> Categories { get; set; }  
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

    }
}

