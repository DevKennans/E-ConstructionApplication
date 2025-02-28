using EConstructionApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EConstructionApp.Persistence.Configurations
{
    public class EmployeeAttendanceConfiguration : IEntityTypeConfiguration<EmployeeAttendance>
    {
        public void Configure(EntityTypeBuilder<EmployeeAttendance> builder)
        {
            builder.HasKey(ea => ea.Id);

            builder.Property(ea => ea.Dairy).IsRequired().HasColumnType("date");
            builder.Property(ea => ea.CheckInTime).HasColumnType("datetime");
            builder.Property(ea => ea.CheckOutTime).HasColumnType("datetime");

            builder.HasOne(ea => ea.Employee)
                .WithMany(e => e.EmployeeAttendances)
                .HasForeignKey(ea => ea.EmployeeId);
        }
    }
}
