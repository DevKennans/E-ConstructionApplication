using EConstructionApp.Domain.Enums;

namespace EConstructionApp.Application.DTOs.Materials
{
    public class MaterialUpdateDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal StockQuantity { get; set; }

        public Measure Measure { get; set; }

        public Guid CategoryId { get; set; }
    }
}
