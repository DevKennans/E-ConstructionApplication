using EConstructionApp.Application.Interfaces.Services.Identification;
using MediatR;

namespace EConstructionApp.Application.Features.Commands.Auth.LogIn
{
    public class LogInCommandHandler : IRequestHandler<LogInCommandRequest, LogInCommandResponse>
    {
        private readonly IAuthService _authService;
        public LogInCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<LogInCommandResponse> Handle(LogInCommandRequest logInCommandRequest, CancellationToken cancellationToken)
        {
            return await _authService.LogInAsync(logInCommandRequest);
        }
    }
}
