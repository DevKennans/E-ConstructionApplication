using EConstructionApp.Application.Features.Commands.Auth.LogIn;
using EConstructionApp.Application.Features.Commands.Auth.SignUp;
using EConstructionApp.Domain.Entities.Identification;

namespace EConstructionApp.Application.Interfaces.Services.Identification
{
    public interface IAuthService
    {
        Task<SignUpCommandResponse> SignUpAsync(SignUpCommandRequest? signUpCommandRequest);

        Task<IList<string>> GetUserAllRoles(AppUser user);

        Task<LogInCommandResponse> LogInAsync(LogInCommandRequest? logInCommandRequest);

        Task<DTOs.Identification.Token?> RefreshTokenAsync(string? refreshToken);

        Task UpdateRefreshToken(string refreshToken, AppUser user, DateTime accessTokenDate, int addOnAccessTokenDate);
    }
}
