using FluentValidation;
using MediatR;
using ValidationException = FluentValidation.ValidationException;

namespace EConstructionApp.Application.Behaviors
{
    public class FluentValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        public FluentValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators ?? throw new ArgumentNullException(nameof(validators));
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                ValidationContext<TRequest> validationContext = new ValidationContext<TRequest>(request);
                List<FluentValidation.Results.ValidationFailure> validationFailures = _validators
                    .Select(validator => validator.Validate(validationContext))
                    .SelectMany(validationResult => validationResult.Errors)
                    .Where(validationFailure => validationFailure is not null)
                    .GroupBy(validationFailure => validationFailure.ErrorMessage)
                    .Select(groupedFailures => groupedFailures.First())
                    .ToList();

                if (validationFailures.Any())
                    throw new ValidationException(validationFailures);
            }

            return await next();
        }
    }
}
