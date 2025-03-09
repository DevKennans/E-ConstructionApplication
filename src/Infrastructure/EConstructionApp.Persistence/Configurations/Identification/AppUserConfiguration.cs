using EConstructionApp.Domain.Entities.Identification;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EConstructionApp.Persistence.Configurations.Identification
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            PasswordHasher<AppUser> hasher = new PasswordHasher<AppUser>();

            AppUser admin = new AppUser
            {
                Id = "43B64316-DE56-4300-AE86-C298AEA73C7B",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                PhoneNumberConfirmed = true
            };
            admin.PasswordHash = hasher.HashPassword(admin, "Admin123!");

            AppUser moderator = new AppUser
            {
                Id = "99D74F43-7E23-41BE-9F98-10C5D6130312",
                UserName = "moderator",
                NormalizedUserName = "MODERATOR",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                PhoneNumberConfirmed = true
            };
            moderator.PasswordHash = hasher.HashPassword(moderator, "Moderator123!");

            builder.HasData(admin, moderator);
        }
    }
}
