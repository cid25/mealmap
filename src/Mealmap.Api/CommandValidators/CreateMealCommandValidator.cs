using Mealmap.Api.Commands;
using Mealmap.Api.CommandValidators;

namespace Mealmap.Api;

public class CreateMealCommandValidator : ICommandValidator<CreateMealCommand>
{
    private readonly MealCommandValidator _mealValidator;

    public CreateMealCommandValidator(MealCommandValidator validations)
    {
        _mealValidator = validations;
    }

    public virtual IReadOnlyCollection<CommandError> Validate(CreateMealCommand command)
    {
        List<CommandError> errors = new();

        return errors
            .PotentiallyWithErrorFrom(_mealValidator.ValidateSingleMainCourseOnly(command.Dto))
            .PotentiallyWithErrorsFrom(_mealValidator.ValidateDishesExist(command.Dto))
            .AsReadOnly();
    }
}
