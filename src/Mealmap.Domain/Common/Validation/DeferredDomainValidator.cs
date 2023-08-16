namespace Mealmap.Domain.Common.Validation;

public class DeferredDomainValidator : IDeferredDomainValidator
{
    private readonly IServiceProvider _provider;

    public DeferredDomainValidator(IServiceProvider provider)
    {
        _provider = provider;
    }

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
        var validator = (IEntityValidator?)_provider.GetService(validatorType);

        return validator;
    }
}
