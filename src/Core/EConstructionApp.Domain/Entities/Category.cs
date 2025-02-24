using EConstructionApp.Domain.Entities.Common;
using EConstructionApp.Domain.Entities.Common.Abstractions;

namespace EConstructionApp.Domain.Entities
{
    public class Category : BaseEntity, IActivityStatus
    {
        public string Name { get; set; }

        public bool IsDeleted { get; set; } = false;

        public ICollection<Material> Materials { get; set; } = new List<Material>();
    }
}
