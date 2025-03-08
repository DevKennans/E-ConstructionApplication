using EConstructionApp.Domain.Entities;
using EConstructionApp.Domain.Enums.Employees;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EConstructionApp.Persistence.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);
            builder.ToTable(tb => tb.HasCheckConstraint("CK_Employee_FirstName_Length", "LEN(FirstName) >= 2"));

            builder.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);
            builder.ToTable(tb => tb.HasCheckConstraint("CK_Employee_LastName_Length", "LEN(LastName) >= 2"));

            builder.Property(e => e.DateOfBirth)
                .IsRequired();
            builder.ToTable(tb => tb.HasCheckConstraint("CK_Employee_Min_Age", "DateOfBirth <= DATEADD(YEAR, -18, GETDATE())"));

            builder.Property(e => e.PhoneNumber)
                .IsRequired()
                .HasMaxLength(15);
            builder.ToTable(tb => tb.HasCheckConstraint("CK_Employee_PhoneNumber_Length", "LEN(PhoneNumber) >= 10"));

            builder.Property(e => e.Address)
                .IsRequired()
                .HasMaxLength(250);
            builder.ToTable(tb => tb.HasCheckConstraint("CK_Employee_Address_Length", "LEN(Address) >= 5"));

            builder.Property(e => e.Salary)
                .IsRequired()
                .HasColumnType("decimal(18,2)");
            builder.ToTable(tb => tb.HasCheckConstraint("CK_Employee_Salary", "Salary >= 0"));

            builder.Property(e => e.Role)
                .IsRequired()
                .HasDefaultValue(EmployeeRole.Laborer);

            builder.Property(e => e.IsCurrentlyWorking)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(e => e.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasOne(e => e.CurrentTask)
                .WithMany(t => t.Employees)
                .HasForeignKey(e => e.CurrentTaskId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
