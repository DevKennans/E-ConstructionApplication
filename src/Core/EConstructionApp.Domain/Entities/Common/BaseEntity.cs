namespace EConstructionApp.Domain.Entities.Common
{
    public class BaseEntity
    {
        public Guid Id { get; set; }

        public DateTime InsertDate { get; set; }
        public virtual DateTime? ModifiedDate { get; set; }
    }
}
