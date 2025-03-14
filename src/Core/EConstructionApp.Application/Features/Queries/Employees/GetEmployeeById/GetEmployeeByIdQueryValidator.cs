using FluentValidation;

namespace EConstructionApp.Application.Features.Queries.Employees.GetEmployeeById
{
    public class GetEmployeeByIdQueryValidator : AbstractValidator<GetEmployeeByIdQueryRequest>
    {
        public GetEmployeeByIdQueryValidator()
        {
            RuleFor(x => x.EmployeeId)
                .NotEmpty().WithMessage("Employee ID is required.")
                .Must(id => id != Guid.Empty).WithMessage("Invalid Employee ID.");
        }
    }
}
