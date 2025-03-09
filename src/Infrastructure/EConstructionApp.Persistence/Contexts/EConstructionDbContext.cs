using EConstructionApp.Domain.Entities;
using EConstructionApp.Domain.Entities.Common;
using EConstructionApp.Domain.Entities.Cross;
using EConstructionApp.Domain.Entities.Identification;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EConstructionApp.Persistence.Contexts
{
    public class EConstructionDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public EConstructionDbContext() { }

        public EConstructionDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Material> Materials { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Domain.Entities.Task> Tasks { get; set; }
        public DbSet<MaterialTask> MaterialTasks { get; set; }
        public DbSet<EmployeeAttendance> EmployeeAttendances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(EConstructionDbContext).Assembly);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<BaseEntity> entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Modified)
                    entry.Entity.ModifiedDate = DateTime.Now;
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
