using EConstructionApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EConstructionApp.Persistence.Configurations
{
    public class MaterialConfiguration : IEntityTypeConfiguration<Material>
    {
        public void Configure(EntityTypeBuilder<Material> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(250);
            builder.ToTable(tb => tb.HasCheckConstraint("CK_Material_Name_Length", "LEN(Name) >= 2"));

            builder.Property(m => m.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");
            builder.ToTable(tb => tb.HasCheckConstraint("CK_Material_Price", "Price > 0"));

            builder.Property(m => m.StockQuantity)
                .IsRequired()
                .HasColumnType("decimal(18,2)");
            builder.ToTable(tb => tb.HasCheckConstraint("CK_Material_StockQuantity", "StockQuantity >= 0"));

            builder.Property(m => m.Measure)
                .IsRequired();

            builder.Property(e => e.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasOne(m => m.Category)
                .WithMany(c => c.Materials)
                .HasForeignKey(m => m.CategoryId);
        }
    }
}
