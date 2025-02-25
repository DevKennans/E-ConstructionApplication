using EConstructionApp.Application.DTOs.Categories;
using EConstructionApp.Domain.Enums;

namespace EConstructionApp.Application.DTOs.Materials
{
    public class MaterialDto
    {
        public Guid Id { get; set; }
        public virtual DateTime InsertedDate { get; set; }
        public virtual DateTime? ModifiedDate { get; set; }

        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal StockQuantity { get; set; }

        public Measure Measure { get; set; }

        public bool IsDeleted { get; set; }

        public CategoryDto Category { get; set; }
    }
}
