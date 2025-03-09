using EConstructionApp.Application.Features.Commands.Auth.SignUp;

namespace EConstructionApp.Application.Interfaces.Services.Identification
{
    public interface IAuthService
    {
        Task<SignUpCommandResponse> SignUpAsync(SignUpCommandRequest? signUpCommandRequest);
    }
}
