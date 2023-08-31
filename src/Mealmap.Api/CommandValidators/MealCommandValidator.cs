using Mealmap.Api.DataTransferObjects;
using Mealmap.Domain.DishAggregate;

namespace Mealmap.Api.CommandValidators;

public class MealCommandValidator
{
    private readonly IDishRepository _repository;

    public MealCommandValidator(IDishRepository repository)
        => _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public CommandError? ValidateSingleMainCourseOnly(MealDTO dto)
    {
        if (dto.Courses != null && dto.Courses.Where(x => x.MainCourse == true).Count() > 1)
            return new CommandError(CommandErrorCodes.NotValid, "There may only be one main course.");

        return null;
    }

    public ICollection<CommandError> ValidateDishesExist(MealDTO dto)
    {
        List<CommandError> errors = new();

        dto.Courses?
            .Select(c => new { Exists = _repository.GetSingleById(c.DishId) != null, Id = c.DishId })
            .Where(c => !c.Exists)
            .ToList()
            .ForEach(f => errors.Add(
                new CommandError(CommandErrorCodes.NotValid, $"Dish with id {f.Id} not found.")));

        return errors;
    }
}
