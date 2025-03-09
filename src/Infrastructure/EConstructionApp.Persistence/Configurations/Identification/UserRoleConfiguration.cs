using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EConstructionApp.Persistence.Configurations.Identification
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
            string adminUserId = "43B64316-DE56-4300-AE86-C298AEA73C7B";
            string moderatorUserId = "99D74F43-7E23-41BE-9F98-10C5D6130312";

            string adminRoleId = "18FF4584-E530-4CFE-ADC1-9485EBCC1982";
            string moderatorRoleId = "BA54FA79-8361-4560-B1A5-C55097FE6E63";

            builder.HasData(
                new IdentityUserRole<string> { UserId = adminUserId, RoleId = adminRoleId },
                new IdentityUserRole<string> { UserId = moderatorUserId, RoleId = moderatorRoleId }
            );
        }
    }
}
