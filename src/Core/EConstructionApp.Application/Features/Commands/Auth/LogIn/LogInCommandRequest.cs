using MediatR;

namespace EConstructionApp.Application.Features.Commands.Auth.LogIn
{
    public class LogInCommandRequest : IRequest<LogInCommandResponse>
    {
        public string UsernameOrPhoneNumber { get; set; }
        public string Password { get; set; }
    }
}
