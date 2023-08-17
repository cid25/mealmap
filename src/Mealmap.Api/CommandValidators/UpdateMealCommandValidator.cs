using Mealmap.Api.Commands;
using Mealmap.Api.CommandValidators;

namespace Mealmap.Api;

public class UpdateMealCommandValidator : ICommandValidator<UpdateMealCommand>
{
    private readonly MealCommandValidations _validations;

    public UpdateMealCommandValidator(MealCommandValidations validations)
    {
        _validations = validations;
    }

    public virtual IReadOnlyCollection<CommandError> Validate(UpdateMealCommand command)
    {
        List<CommandError> errors = new();

        return errors
            .PotentiallyWithErrorFrom(_validations.ValidateSingleMainCourseOnly(command.Dto))
            .PotentiallyWithErrorsFrom(_validations.ValidateDishesExist(command.Dto))
            .AsReadOnly();
    }
}
