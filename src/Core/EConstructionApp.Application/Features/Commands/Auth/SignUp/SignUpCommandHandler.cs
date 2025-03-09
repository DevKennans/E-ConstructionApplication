using EConstructionApp.Application.Interfaces.Services.Identification;
using MediatR;

namespace EConstructionApp.Application.Features.Commands.Auth.SignUp
{
    public class SignUpCommandHandler : IRequestHandler<SignUpCommandRequest, SignUpCommandResponse>
    {
        private readonly IAuthService _authService;
        public SignUpCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<SignUpCommandResponse> Handle(SignUpCommandRequest signUpCommandRequest, CancellationToken cancellationToken)
        {
            return await _authService.SignUpAsync(signUpCommandRequest);
        }
    }
}
