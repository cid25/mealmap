namespace Mealmap.Domain.Seedwork.Validation;

public interface IEntityValidator<TEntity>
{
    public Task<DomainValidationResult> ValidateAsync(TEntity entity);
}
