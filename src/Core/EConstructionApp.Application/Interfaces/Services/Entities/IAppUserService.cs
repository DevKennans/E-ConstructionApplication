namespace EConstructionApp.Application.Interfaces.Services.Entities
{
    public interface IAppUserService
    {
        Task<(bool IsSuccess, string Message)> UpdatePasswordAsync(string userId, string newPassword, string confirmNewPassword);
    }
}
