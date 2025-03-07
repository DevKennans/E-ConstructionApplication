using EConstructionApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EConstructionApp.Persistence.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(250);
            builder.ToTable(tb => tb.HasCheckConstraint("CK_Category_Name_Length", "LEN(Name) >= 2"));

            builder.Property(e => e.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);
        }
    }
}
