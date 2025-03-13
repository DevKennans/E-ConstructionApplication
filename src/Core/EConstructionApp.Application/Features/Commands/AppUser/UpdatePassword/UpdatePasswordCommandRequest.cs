using MediatR;

namespace EConstructionApp.Application.Features.Commands.AppUser.UpdatePassword
{
    public class UpdatePasswordCommandRequest : IRequest<UpdatePasswordCommandResponse>
    {
        public string AppUserId { get; set; }

        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}
