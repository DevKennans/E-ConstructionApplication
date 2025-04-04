using EConstructionApp.Domain.Enums;
using EConstructionApp.Domain.Enums.Materials;

namespace EConstructionApp.Application.DTOs.Materials.Relations;

public class MaterialTransactionLogDto
{
    public Guid TaskId { get; set; }
    public Guid MaterialId { get; set; }

    public string MaterialName { get; set; }
    public string CategoryName { get; set; }

    public decimal Quantity { get; set; }
    public Measure Measure { get; set; }

    public decimal PriceAtTransaction { get; set; }
    public MaterialTransactionType TransactionType { get; set; }
}