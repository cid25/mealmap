using Mealmap.Domain.Common;
using Mealmap.Domain.MealAggregate;
using Mealmap.Domain.Seedwork.Validation;
using MediatR;

namespace Mealmap.Domain;

public class DeferredDomainValidator : IDeferredDomainValidator
{
    private readonly IMediator _mediator;

    public DeferredDomainValidator(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task ValidateEntitiesAsync(IReadOnlyCollection<EntityBase> entities)
    {
        if (entities != null)
        {
            foreach (var entity in entities)
            {
                DomainValidationResult? result = null;

                result = entity switch
                {
                    Meal meal => await _mediator.Send(new ValidationRequest<Meal>(meal)),
                    _ => null
                };

                if (result != null && !result.IsValid)
                    throw new DomainValidationException($"{entity.GetType} with Id {entity.Id} is invalid.");
            }
        }

        return;
    }
}
