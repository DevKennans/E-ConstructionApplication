using EConstructionApp.Application.Interfaces.Services.Entities;
using EConstructionApp.Domain.Entities.Identification;
using Microsoft.AspNetCore.Identity;

namespace EConstructionApp.Persistence.Concretes.Services.Entities
{
    public class AppUserService : IAppUserService
    {
        private readonly UserManager<AppUser> _userManager;
        public AppUserService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<(bool IsSuccess, string Message)> UpdatePasswordAsync(string userId, string newPassword, string confirmNewPassword)
        {
            AppUser? user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return (false, "User not found.");

            string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            IdentityResult result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
            if (!result.Succeeded)
                return (false, string.Join(" ", result.Errors.Select(e => e.Description)));

            return (true, "Password updated successfully.");
        }
    }
}
