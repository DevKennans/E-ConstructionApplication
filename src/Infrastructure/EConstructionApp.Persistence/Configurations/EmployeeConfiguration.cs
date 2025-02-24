using EConstructionApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EConstructionApp.Persistence.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            builder.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            builder.Property(e => e.DateOfBirth).IsRequired();
            builder.Property(e => e.PhoneNumber).HasMaxLength(15);
            builder.Property(e => e.Address).HasMaxLength(250);
            builder.Property(e => e.Salary).IsRequired();
            builder.Property(e => e.IsDeleted).IsRequired();
            builder.Property(e => e.IsCurrentlyWorking).IsRequired();

            builder.HasOne(e => e.CurrentTask)
                .WithMany(t => t.Employees)
                .HasForeignKey(e => e.CurrentTaskId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
