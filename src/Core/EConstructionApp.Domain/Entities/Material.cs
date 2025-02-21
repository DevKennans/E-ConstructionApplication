using EConstructionApp.Domain.Entities.Common;
using EConstructionApp.Domain.Entities.Common.Abstractions;
using EConstructionApp.Domain.Entities.Cross;
using EConstructionApp.Domain.Enums;

namespace EConstructionApp.Domain.Entities
{
    public class Material : BaseEntity, IActivityStatus
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal StockQuantity { get; set; }

        public Measure Measure { get; set; }

        public bool IsDeleted { get; set; } = false;

        public Guid CategoryId { get; set; }
        public Category Category { get; set; }

        public ICollection<MaterialTask> MaterialTasks { get; set; }
    }
}
