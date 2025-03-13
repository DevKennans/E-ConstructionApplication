namespace EConstructionApp.Application.Features.Commands.AppUser.UpdatePassword
{
    public class UpdatePasswordCommandResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        public UpdatePasswordCommandResponse(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }
    }
}
