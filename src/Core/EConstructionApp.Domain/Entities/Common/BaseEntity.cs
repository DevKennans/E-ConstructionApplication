using EConstructionApp.Domain.Entities.Common.Abstractions;

namespace EConstructionApp.Domain.Entities.Common
{
    public class BaseEntity : IBaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public virtual DateTime InsertedDate { get; set; } = DateTime.Now;
        public virtual DateTime? ModifiedDate { get; set; }
    }
}
