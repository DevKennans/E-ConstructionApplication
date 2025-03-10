using FluentValidation;
using System.Text.RegularExpressions;

namespace EConstructionApp.Application.Features.Commands.Auth.LogIn
{
    public class LogInCommandValidator : AbstractValidator<LogInCommandRequest>
    {
        public LogInCommandValidator()
        {
            RuleFor(x => x.UsernameOrPhoneNumber)
                .NotEmpty().WithMessage("Username or phone number is required.")
                .Must(BeValidUsernameOrPhone).WithMessage("Invalid username or phone number format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"\d").WithMessage("Password must contain at least one digit.")
                .Matches(@"[^\w\d]").WithMessage("Password must contain at least one special character.");
        }

        private bool BeValidUsernameOrPhone(string value)
        {
            return Regex.IsMatch(value, @"^\d{10,15}$") || Regex.IsMatch(value, @"^[a-zA-Z0-9._-]{3,20}$");
        }
    }
}
