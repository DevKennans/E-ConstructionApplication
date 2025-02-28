using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskStatus = EConstructionApp.Domain.Enums.Tasks.TaskStatus;

namespace EConstructionApp.Persistence.Configurations
{
    public class TaskConfiguration : IEntityTypeConfiguration<Domain.Entities.Task>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Task> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.AssignedBy).IsRequired().HasMaxLength(200);
            builder.Property(t => t.AssignedByPhone).IsRequired().HasMaxLength(15);
            builder.Property(t => t.AssignedByEmail).IsRequired().HasMaxLength(150);
            builder.Property(t => t.AssignedByAddress).IsRequired().HasMaxLength(250);

            builder.Property(t => t.Title).IsRequired().HasMaxLength(250);
            builder.Property(t => t.Description).IsRequired().HasMaxLength(1000);
            builder.Property(t => t.Deadline).IsRequired();

            builder.Property(t => t.Priority).IsRequired();
            builder.Property(t => t.Status).IsRequired().HasDefaultValue(TaskStatus.Pending);

            builder.Property(t => t.IsDeleted).IsRequired().HasDefaultValue(false);
        }
    }
}
