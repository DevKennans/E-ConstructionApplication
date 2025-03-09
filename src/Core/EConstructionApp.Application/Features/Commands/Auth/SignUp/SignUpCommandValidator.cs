using FluentValidation;

namespace EConstructionApp.Application.Features.Commands.Auth.SignUp
{
    public class SignUpCommandValidator : AbstractValidator<SignUpCommandRequest>
    {
        public SignUpCommandValidator()
        {
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\d{10,15}$").WithMessage("Phone number must be numeric and between 10-15 characters.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"\d").WithMessage("Password must contain at least one digit.")
                .Matches(@"[^\w\d]").WithMessage("Password must contain at least one special character.");

            RuleFor(x => x.PasswordConfirmation)
                .Equal(x => x.Password).WithMessage("Password confirmation does not match.");
        }
    }
}
