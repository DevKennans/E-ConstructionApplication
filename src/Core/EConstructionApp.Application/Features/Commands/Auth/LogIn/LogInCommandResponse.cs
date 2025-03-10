namespace EConstructionApp.Application.Features.Commands.Auth.LogIn
{
    public class LogInCommandResponse
    {
        public DTOs.Identification.Token? Token { get; set; }

        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
