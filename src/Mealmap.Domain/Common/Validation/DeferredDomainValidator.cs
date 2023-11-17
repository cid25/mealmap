namespace Mealmap.Domain.Common.Validation;

/// <inheritdoc cref="IDeferredDomainValidator" />
public class DeferredDomainValidator(IServiceProvider provider) : IDeferredDomainValidator
{
    public async Task ValidateEntitiesAsync(IReadOnlyCollection<EntityBase> entities)
    {
        foreach (var entity in entities)
        {
            var validator = GetValidator(entity);

            if (validator != null)
            {
                var result = await validator.ValidateEntityAsync(entity);

                if (!result.IsValid)
                    throw new DomainValidationException($"{entity.GetType} with Id {entity.Id} is invalid.");
            }
        }
    }

    private IEntityValidator? GetValidator<TEntity>(TEntity entity)
    {
        var entityType = entity!.GetType();
        var validatorType = typeof(AbstractEntityValidator<>).MakeGenericType(entityType);
        var validator = (IEntityValidator?)provider.GetService(validatorType);

        return validator;
    }
}
