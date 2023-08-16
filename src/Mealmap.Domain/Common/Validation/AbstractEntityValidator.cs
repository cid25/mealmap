using Mealmap.Domain.Common;

namespace Mealmap.Domain.Common.Validation;

public abstract class AbstractEntityValidator<TEntity> : IEntityValidator
    where TEntity : EntityBase
{
    public async Task<DomainValidationResult> ValidateEntityAsync(EntityBase entity)
        => await ValidateAsync((TEntity)entity);

    public abstract Task<DomainValidationResult> ValidateAsync(TEntity entity);
}
