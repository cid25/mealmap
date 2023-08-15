using Mealmap.Domain.Common;

namespace Mealmap.Domain.Seedwork.Validation;

public interface IEntityValidator
{
    public Task<DomainValidationResult> ValidateEntityAsync(EntityBase entity);
}
