using Mealmap.Domain.DishAggregate;
using Mealmap.Domain.Seedwork.Validation;

namespace Mealmap.Domain.MealAggregate;

public class MealValidator : IEntityValidator<Meal>
{
    private readonly IDishRepository _repository;

    public MealValidator(IDishRepository repository)
    {
        _repository = repository;
    }

    public Task<DomainValidationResult> ValidateAsync(Meal entity)
    {
        DomainValidationResult result = new();

        if (entity.Courses.Any())
            foreach (var course in entity.Courses)
                if (_repository.GetSingleById(course.DishId) == null)
                    result.AddError($"Dish with Id {course.DishId} not found.");

        return Task.FromResult(result);
    }
}
