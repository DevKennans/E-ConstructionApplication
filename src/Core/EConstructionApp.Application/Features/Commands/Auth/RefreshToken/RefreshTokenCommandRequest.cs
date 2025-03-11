using MediatR;

namespace EConstructionApp.Application.Features.Commands.Auth.RefreshToken
{
    public class RefreshTokenCommandRequest : IRequest<RefreshTokenCommandResponse>
    {
        public string? RefreshToken { get; set; }
    }
}
