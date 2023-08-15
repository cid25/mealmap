using Mealmap.Domain.Common;
using Mealmap.Domain.Seedwork.Validation;

namespace Mealmap.Domain.UnitTests;

internal class DummyEntity : EntityBase
{
    public bool IsValid { get; set; }

    public DummyEntity(bool isValid)
    {
        this.IsValid = isValid;
    }
}

internal class DummyEntityValidator : AbstractEntityValidator<DummyEntity>
{
    public override Task<DomainValidationResult> ValidateAsync(DummyEntity entity)
    {
        DomainValidationResult result = new();

        if (!entity.IsValid)
            result.AddError("Dummy invalid.");

        return Task.FromResult(result);
    }
}
