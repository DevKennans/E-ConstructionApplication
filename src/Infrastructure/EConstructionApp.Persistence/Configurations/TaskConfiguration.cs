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

            builder.Property(t => t.AssignedBy)
                .IsRequired()
                .HasMaxLength(200);
            builder.ToTable(tb => tb.HasCheckConstraint("CK_Task_AssignedBy_Length", "LEN(AssignedBy) >= 2"));

            builder.Property(t => t.AssignedByPhone)
                .IsRequired()
                .HasMaxLength(15);
            builder.ToTable(tb => tb.HasCheckConstraint("CK_Task_AssignedByPhone_Length", "LEN(AssignedByPhone) >= 10"));

            builder.Property(t => t.AssignedByAddress)
                .IsRequired()
                .HasMaxLength(250);
            builder.ToTable(tb => tb.HasCheckConstraint("CK_Task_AssignedByAddress_Length", "LEN(AssignedByAddress) >= 5"));

            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(250);
            builder.ToTable(tb => tb.HasCheckConstraint("CK_Task_Title_Length", "LEN(Title) >= 5"));

            builder.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(1000);
            builder.ToTable(tb => tb.HasCheckConstraint("CK_Task_Description_Length", "LEN(Description) >= 10"));

            builder.Property(t => t.Deadline)
                .IsRequired();
            builder.ToTable(tb => tb.HasCheckConstraint("CK_Task_Deadline", "Deadline >= DATEADD(DAY, 1, GETDATE())"));

            builder.Property(t => t.Priority)
                .IsRequired();

            builder.Property(t => t.Status)
                .IsRequired()
                .HasDefaultValue(TaskStatus.Pending);

            builder.Property(t => t.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);
        }
    }
}
