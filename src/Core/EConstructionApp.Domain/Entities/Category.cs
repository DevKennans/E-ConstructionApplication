using EConstructionApp.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace EConstructionApp.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }

        [NotMapped]
        public override DateTime? ModifiedDate { get => base.ModifiedDate; set => base.ModifiedDate = value; }

        public ICollection<Material> Materials { get; set; }
    }
}
