using EConstructionApp.Domain.Entities.Common.Abstractions;

namespace EConstructionApp.Domain.Entities.Cross
{
    public class MaterialTask : IBaseEntity
    {
        public Guid MaterialId { get; set; }
        public Material Material { get; set; }

        public decimal Quantity { get; set; }

        public Guid TaskId { get; set; }
        public Task Task { get; set; }
    }
}
