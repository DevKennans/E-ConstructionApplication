using FluentValidation;

namespace EConstructionApp.Application.Features.Commands.Employees.EmployeeCheckInOut
{
    public class EmployeeCheckInOutCommandValidator : AbstractValidator<EmployeeCheckInOutCommandRequest>
    {
        public EmployeeCheckInOutCommandValidator()
        {
            RuleFor(x => x.EmployeeId)
                .NotEmpty().WithMessage("Employee ID is required.")
                .Must(id => id != Guid.Empty).WithMessage("Invalid Employee ID.");

            RuleFor(x => x.ScanTime)
                .NotEmpty().WithMessage("Scan time is required.")
                .Must(BeValidDateTime).WithMessage("Scan time must be a valid date and time.");
        }

        private bool BeValidDateTime(DateTime scanTime)
        {
            return scanTime > DateTime.MinValue && scanTime < DateTime.MaxValue;
        }
    }
}
