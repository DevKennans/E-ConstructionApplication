using EConstructionApp.Application.Interfaces.Services.Entities;
using MediatR;

namespace EConstructionApp.Application.Features.Commands.AppUser.UpdatePassword
{
    public class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommandRequest, UpdatePasswordCommandResponse>
    {
        private readonly IAppUserService _appUserService;
        public UpdatePasswordCommandHandler(IAppUserService appUserService)
        {
            _appUserService = appUserService;
        }

        public async Task<UpdatePasswordCommandResponse> Handle(UpdatePasswordCommandRequest updatePasswordCommandRequest, CancellationToken cancellationToken)
        {
            (bool IsSuccess, string Message) = await _appUserService.UpdatePasswordAsync(updatePasswordCommandRequest.AppUserId, updatePasswordCommandRequest.NewPassword, updatePasswordCommandRequest.ConfirmNewPassword);
            return new UpdatePasswordCommandResponse(IsSuccess, Message);
        }
    }
}
