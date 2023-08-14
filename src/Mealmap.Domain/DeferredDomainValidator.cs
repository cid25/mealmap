using Mealmap.Domain.Common;
using Mealmap.Domain.MealAggregate;
using Mealmap.Domain.Seedwork.Validation;

namespace Mealmap.Domain;

public class DeferredDomainValidator : IDeferredDomainValidator
{
    private readonly IServiceProvider _provider;

    public DeferredDomainValidator(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task ValidateEntitiesAsync(IReadOnlyCollection<EntityBase> entities)
    {
        if (entities != null)
        {
            foreach (var entity in entities)
            {
                DomainValidationResult? result = null;

                var validatorType = typeof(IEntityValidator<>).MakeGenericType(entity.GetType());
                dynamic? validator = _provider.GetService(validatorType);

                if (validator != null)
                {
                    result = entity switch
                    {
                        Meal meal => await validator.ValidateAsync(meal),
                        _ => null
                    };
                }

                if (result != null && !result.IsValid)
                    throw new DomainValidationException($"{entity.GetType} with Id {entity.Id} is invalid.");
            }
        }

        return;
    }
}
