using EConstructionApp.Domain.Entities.Common;
using EConstructionApp.Domain.Entities.Common.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;

namespace EConstructionApp.Domain.Entities
{
    public class Category : BaseEntity, IActivityStatus
    {
        public string Name { get; set; }

        public bool IsDeleted { get; set; } = false;

        [NotMapped]
        public override DateTime? ModifiedDate { get => base.ModifiedDate; set => base.ModifiedDate = value; }

        public ICollection<Material> Materials { get; set; } = new List<Material>();
    }
}
