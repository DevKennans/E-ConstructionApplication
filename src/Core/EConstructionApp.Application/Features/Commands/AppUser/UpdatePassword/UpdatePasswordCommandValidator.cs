using FluentValidation;

namespace EConstructionApp.Application.Features.Commands.AppUser.UpdatePassword
{
    public class UpdatePasswordCommandValidator : AbstractValidator<UpdatePasswordCommandRequest>
    {
        public UpdatePasswordCommandValidator()
        {
            RuleFor(x => x.AppUserId)
                .NotEmpty().WithMessage("User ID is required.")
                .Must(BeAValidGuid).WithMessage("User ID must be a valid GUID.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"\d").WithMessage("Password must contain at least one digit.")
                .Matches(@"[^\w\d]").WithMessage("Password must contain at least one special character.");

            RuleFor(x => x.ConfirmNewPassword)
                .Equal(x => x.NewPassword).WithMessage("Password confirmation does not match.");
        }

        private bool BeAValidGuid(string appUserId)
        {
            return Guid.TryParse(appUserId, out _);
        }
    }
}
