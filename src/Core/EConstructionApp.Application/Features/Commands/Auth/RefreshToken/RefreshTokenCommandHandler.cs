using EConstructionApp.Application.Interfaces.Services.Identification;
using MediatR;

namespace EConstructionApp.Application.Features.Commands.Auth.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommandRequest, RefreshTokenCommandResponse>
    {
        private readonly IAuthService _authService;
        public RefreshTokenCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<RefreshTokenCommandResponse> Handle(RefreshTokenCommandRequest refreshTokenCommandRequest, CancellationToken cancellationToken)
        {
            return new RefreshTokenCommandResponse
            {
                Token = await _authService.RefreshTokenAsync(refreshTokenCommandRequest.RefreshToken)
            };
        }
    }
}
