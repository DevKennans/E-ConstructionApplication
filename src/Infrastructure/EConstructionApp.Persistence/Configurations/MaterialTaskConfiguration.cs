using EConstructionApp.Domain.Entities.Cross;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EConstructionApp.Persistence.Configurations
{
    public class MaterialTaskConfiguration : IEntityTypeConfiguration<MaterialTask>
    {
        public void Configure(EntityTypeBuilder<MaterialTask> builder)
        {
            builder.HasKey(mt => new { mt.MaterialId, mt.TaskId });

            builder.HasOne(mt => mt.Material)
                .WithMany(m => m.MaterialTasks)
                .HasForeignKey(mt => mt.MaterialId);

            builder.HasOne(mt => mt.Task)
                .WithMany(t => t.MaterialTasks)
                .HasForeignKey(mt => mt.TaskId);

            builder.Property(mt => mt.Quantity)
                .IsRequired()
                .HasColumnType("decimal(18,2)");
        }
    }
}
