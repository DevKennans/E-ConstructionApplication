using EConstructionApp.Domain.Entities.Common;
using EConstructionApp.Domain.Enums;
using EConstructionApp.Domain.Enums.Materials;
using System.ComponentModel.DataAnnotations.Schema;

namespace EConstructionApp.Domain.Entities.Relations
{
    public class MaterialTransactionLog : BaseEntity
    {
        public Guid TaskId { get; set; }
        public Guid MaterialId { get; set; }

        public decimal Quantity { get; set; }
        public Measure Measure { get; set; }
        public decimal PriceAtTransaction { get; set; }

        public MaterialTransactionType TransactionType { get; set; }

        [NotMapped] public override DateTime? ModifiedDate { get; set; }
    }
}