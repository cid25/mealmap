using FluentValidation;

namespace Mealmap.Api.Dishes;

public class DishDataTransferObjectValidator : AbstractValidator<DishDTO>
{
    public DishDataTransferObjectValidator()
    {
        RuleFor(dto => dto.Name).MinimumLength(3).MaximumLength(50)
            .WithMessage("The length of the name be between 3 and 50, but is {PropertyValue}.");

        RuleFor(dto => dto.Description).MaximumLength(80)
            .WithMessage("The length of the description cannot be greather than 80, but is {PropertyValue}.");

        RuleForEach(dto => dto.Ingredients).ChildRules(ingredient =>
        {
            ingredient.RuleFor(i => i.Quantity).GreaterThan(0m)
                .WithMessage("The quantity of an ingredient must be greater than zero, but is {PropertyValue}.");
        });
    }
}
