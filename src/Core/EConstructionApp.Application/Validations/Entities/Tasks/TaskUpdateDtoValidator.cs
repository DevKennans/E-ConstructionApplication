using EConstructionApp.Application.DTOs.Tasks;
using FluentValidation;

namespace EConstructionApp.Application.Validations.Entities.Tasks
{
    public class TaskUpdateDtoValidator : AbstractValidator<TaskDetailsUpdateDto>
    {
        public TaskUpdateDtoValidator()
        {
            RuleFor(t => t.AssignedBy)
            .NotEmpty().WithMessage("AssignedBy is required.")
            .MinimumLength(2).WithMessage("AssignedBy must be at least 2 characters long.")
            .MaximumLength(200).WithMessage("AssignedBy cannot exceed 200 characters.");

            RuleFor(t => t.AssignedByPhone)
                .NotEmpty().WithMessage("AssignedByPhone is required.")
                .MinimumLength(10).WithMessage("AssignedByPhone must be at least 10 characters long.")
                .MaximumLength(15).WithMessage("AssignedByPhone cannot exceed 15 characters.");

            RuleFor(t => t.AssignedByAddress)
                .NotEmpty().WithMessage("AssignedByAddress is required.")
                .MinimumLength(5).WithMessage("AssignedByAddress must be at least 5 characters long.")
                .MaximumLength(250).WithMessage("AssignedByAddress cannot exceed 250 characters.");

            RuleFor(t => t.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MinimumLength(5).WithMessage("Title must be at least 5 characters long.")
                .MaximumLength(250).WithMessage("Title cannot exceed 250 characters.");

            RuleFor(t => t.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MinimumLength(10).WithMessage("Description must be at least 10 characters long.")
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

            RuleFor(t => t.Deadline)
                .NotEmpty().WithMessage("Deadline is required.")
                .Must(d => d > DateOnly.FromDateTime(DateTime.Today))
                .WithMessage("Deadline must be at least tomorrow.");

            RuleFor(t => t.Priority)
                .IsInEnum().WithMessage("Invalid priority value.");

            RuleFor(t => t.Status)
                .IsInEnum().WithMessage("Invalid status value.");
        }
    }
}
