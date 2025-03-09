using EConstructionApp.Domain.Entities.Identification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EConstructionApp.Persistence.Configurations.Identification
{
    public class AppRoleConfiguration : IEntityTypeConfiguration<AppRole>
    {
        public void Configure(EntityTypeBuilder<AppRole> builder)
        {
            List<AppRole> roles = new List<AppRole>
        {
            new AppRole { Id = "18FF4584-E530-4CFE-ADC1-9485EBCC1982", Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = Guid.NewGuid().ToString() },
            new AppRole { Id = "BA54FA79-8361-4560-B1A5-C55097FE6E63", Name = "Moderator", NormalizedName = "MODERATOR", ConcurrencyStamp = Guid.NewGuid().ToString() },
            new AppRole { Id = "A6975C7C-5397-4BBB-B450-2790781BCBAB", Name = "Employee", NormalizedName = "EMPLOYEE", ConcurrencyStamp = Guid.NewGuid().ToString() }
        };

            builder.HasData(roles);
        }
    }
}
