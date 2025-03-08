using EConstructionApp.Application.DTOs.Employees;
using FluentValidation;

namespace EConstructionApp.Application.Validations.Entities.Employees
{
    public class EmployeeUpdateDtoValidator : AbstractValidator<EmployeeUpdateDto>
    {
        public EmployeeUpdateDtoValidator()
        {
            RuleFor(e => e.Id)
                .NotEmpty().WithMessage("Employee ID is required.");

            RuleFor(e => e.FirstName)
                .NotEmpty().WithMessage("First name cannot be empty or whitespace.")
                .Matches(@"^\S.*$").WithMessage("First name cannot be only whitespace.")
                .Length(2, 100).WithMessage("First name must be between 2 and 100 characters.");

            RuleFor(e => e.LastName)
                .NotEmpty().WithMessage("Last name cannot be empty or whitespace.")
                .Matches(@"^\S.*$").WithMessage("Last name cannot be only whitespace.")
                .Length(2, 100).WithMessage("Last name must be between 2 and 100 characters.");

            RuleFor(e => e.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required.")
                .Must(date => date <= DateOnly.FromDateTime(DateTime.Today.AddYears(-18)))
                .WithMessage("Employee must be at least 18 years old.");

            RuleFor(e => e.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\d{10,15}$").WithMessage("Phone number must contain only digits and be between 10 and 15 characters.");

            RuleFor(e => e.Address)
                .NotEmpty().WithMessage("Address is required.")
                .MinimumLength(5).WithMessage("Address must be at least 5 characters long.")
                .MaximumLength(250).WithMessage("Address cannot exceed 250 characters.");

            RuleFor(e => e.Salary)
                .GreaterThanOrEqualTo(0).WithMessage("Salary cannot be negative.");

            RuleFor(e => e.Role)
                .IsInEnum().WithMessage("Invalid role selected.");
        }
    }
}
