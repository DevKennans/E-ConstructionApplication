using EConstructionApp.Domain.Entities;
using EConstructionApp.Domain.Entities.Cross;
using Microsoft.EntityFrameworkCore;

namespace EConstructionApp.Persistence.Contexts
{
    public class EConstructionDbContext : DbContext
    {
        protected EConstructionDbContext() { }

        public EConstructionDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Material> Materials { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Domain.Entities.Task> Tasks { get; set; }
        public DbSet<EmployeeTask> EmployeeTasks { get; set; }
        public DbSet<MaterialTask> MaterialTasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Material>(builder =>
            {
                builder.HasKey(m => m.Id);
                builder.Property(m => m.Name).IsRequired().HasMaxLength(150);
                builder.Property(m => m.Price).IsRequired().HasColumnType("decimal(18,2)");
                builder.Property(m => m.StockQuantity).IsRequired();
                builder.Property(m => m.Measure).IsRequired();
                builder.HasOne(m => m.Category)
                    .WithMany(c => c.Materials)
                    .HasForeignKey(m => m.CategoryId);

                builder.HasMany(m => m.MaterialTasks)
                    .WithOne(mt => mt.Material)
                    .HasForeignKey(mt => mt.MaterialId);
            });

            modelBuilder.Entity<Category>(builder =>
            {
                builder.HasKey(c => c.Id);
                builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<Employee>(builder =>
            {
                builder.HasKey(e => e.Id);
                builder.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                builder.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                builder.Property(e => e.DateOfBirth).IsRequired();
                builder.Property(e => e.PhoneNumber).HasMaxLength(15);
                builder.Property(e => e.Address).HasMaxLength(250);
                builder.Property(e => e.IsActiveEmployee).IsRequired();

                builder.HasMany(e => e.EmployeeTasks)
                    .WithOne(et => et.Employee)
                    .HasForeignKey(et => et.EmployeeId);

                builder.HasMany(e => e.EmployeeAttendances)
                .WithOne(ea => ea.Employee)
                .HasForeignKey(ea => ea.EmployeeId);
            });

            modelBuilder.Entity<EmployeeAttendance>(builder =>
            {
                builder.HasKey(ea => ea.Id);
                builder.Property(ea => ea.Date).IsRequired();
                builder.Property(ea => ea.CheckInTime).HasColumnType("datetime");
                builder.Property(ea => ea.CheckOutTime).HasColumnType("datetime");

                builder.HasOne(ea => ea.Employee)
                    .WithMany(e => e.EmployeeAttendances)
                    .HasForeignKey(ea => ea.EmployeeId);
            });

            modelBuilder.Entity<Domain.Entities.Task>(builder =>
            {
                builder.HasKey(t => t.Id);
                builder.Property(t => t.Description).HasMaxLength(500);

                builder.HasMany(t => t.EmployeeTasks)
                    .WithOne(et => et.Task)
                    .HasForeignKey(et => et.TaskId);

                builder.HasMany(t => t.MaterialTasks)
                    .WithOne(mt => mt.Task)
                    .HasForeignKey(mt => mt.TaskId);
            });

            modelBuilder.Entity<EmployeeTask>(builder =>
            {
                builder.HasKey(et => new { et.EmployeeId, et.TaskId });
                builder.HasOne(et => et.Employee)
                    .WithMany(e => e.EmployeeTasks)
                    .HasForeignKey(et => et.EmployeeId);
                builder.HasOne(et => et.Task)
                    .WithMany(t => t.EmployeeTasks)
                    .HasForeignKey(et => et.TaskId);
            });

            modelBuilder.Entity<MaterialTask>(builder =>
            {
                builder.HasKey(mt => new { mt.MaterialId, mt.TaskId });
                builder.HasOne(mt => mt.Material)
                    .WithMany(m => m.MaterialTasks)
                    .HasForeignKey(mt => mt.MaterialId);
                builder.HasOne(mt => mt.Task)
                    .WithMany(t => t.MaterialTasks)
                    .HasForeignKey(mt => mt.TaskId);
            });
        }
    }
}
