using EConstructionApp.Application.DTOs.Materials;
using FluentValidation;

namespace EConstructionApp.Application.Validations.Entities.Materials
{
    public class MaterialUpdateDtoValidator : AbstractValidator<MaterialUpdateDto>
    {
        public MaterialUpdateDtoValidator()
        {
            RuleFor(m => m.Name)
                .NotEmpty().WithMessage("Material name cannot be empty or whitespace.")
                .Matches(@"^\S.*$").WithMessage("Material name cannot be only whitespace.")
                .Length(2, 250).WithMessage("Material name must be between 2 and 250 characters.");

            RuleFor(m => m.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");

            RuleFor(m => m.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative.");

            RuleFor(m => m.CategoryId)
                .NotEmpty().WithMessage("Category is required.");
        }
    }
}
