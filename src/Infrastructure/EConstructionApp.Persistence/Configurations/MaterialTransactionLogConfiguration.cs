using EConstructionApp.Domain.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EConstructionApp.Persistence.Configurations
{
    public class MaterialTransactionLogConfiguration : IEntityTypeConfiguration<MaterialTransactionLog>
    {
        public void Configure(EntityTypeBuilder<MaterialTransactionLog> builder)
        {
            builder.HasKey(mtl => mtl.Id);

            builder.Property(mtl => mtl.TaskId)
                .IsRequired();

            builder.Property(mtl => mtl.MaterialId)
                .IsRequired();

            builder.Property(mtl => mtl.Quantity)
                .IsRequired()
                .HasColumnType("decimal(18,2)");
            builder.ToTable(tb => tb.HasCheckConstraint("CK_MaterialTransactionLog_Quantity", "Quantity > 0"));

            builder.Property(mtl => mtl.Measure)
                .IsRequired();

            builder.Property(mtl => mtl.PriceAtTransaction)
                .IsRequired()
                .HasColumnType("decimal(18,2)");
            builder.ToTable(tb => tb.HasCheckConstraint("CK_MaterialTransactionLog_Price", "PriceAtTransaction >= 0"));

            builder.Property(mtl => mtl.TransactionType)
                .IsRequired()
                .HasConversion<int>();
        }
    }
}