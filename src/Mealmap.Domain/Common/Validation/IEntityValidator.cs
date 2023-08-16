namespace Mealmap.Domain.Common.Validation;

public interface IEntityValidator
{
    public Task<DomainValidationResult> ValidateEntityAsync(EntityBase entity);
}
