using MediatR;

namespace EConstructionApp.Application.Features.Commands.Auth.SignUp
{
    public class SignUpCommandRequest : IRequest<SignUpCommandResponse>
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }
    }
}
