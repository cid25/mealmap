using FluentValidation;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Domain.DishAggregate;

namespace Mealmap.Api.CommandValidators;

public class DishDataTransferObjectValidator : AbstractValidator<DishDTO>
{
    public DishDataTransferObjectValidator()
    {
        RuleForEach(dto => dto.Ingredients).ChildRules(ingredient =>
        {
            ingredient.RuleFor(i => i.Quantity).GreaterThan(0m)
                .WithMessage($"The quantity of an ingredient must be greater than zero.");

            ingredient.RuleFor(i => i.UnitOfMeasurement).Must(unitOfMeasurement =>
                Enum.IsDefined(typeof(UnitOfMeasurementCodes), unitOfMeasurement))
                .WithMessage("The unit of measurement does not match one of the predefined ones.");
        });
    }
}
