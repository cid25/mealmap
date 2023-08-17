using Mealmap.Api.Commands;
using Mealmap.Api.CommandValidators;

namespace Mealmap.Api;

public class CreateMealCommandValidator : ICommandValidator<CreateMealCommand>
{
    private readonly MealCommandValidations _validations;

    public CreateMealCommandValidator(MealCommandValidations validations)
    {
        _validations = validations;
    }

    public virtual IReadOnlyCollection<CommandError> Validate(CreateMealCommand command)
    {
        List<CommandError> errors = new();

        return errors
            .PotentiallyWithErrorFrom(_validations.ValidateSingleMainCourseOnly(command.Dto))
            .PotentiallyWithErrorsFrom(_validations.ValidateDishesExist(command.Dto))
            .AsReadOnly();
    }
}
