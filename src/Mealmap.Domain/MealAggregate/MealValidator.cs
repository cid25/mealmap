using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.Common.Validation;
using Mealmap.Domain.DishAggregate;

namespace Mealmap.Domain.MealAggregate;

public class MealValidator : AbstractEntityValidator<Meal>
{
    private readonly IRepository<Dish> _repository;

    public MealValidator(IRepository<Dish> repository)
    {
        _repository = repository;
    }

    public override Task<DomainValidationResult> ValidateAsync(Meal entity)
    {
        DomainValidationResult result = new();

        if (entity.Courses.Any())
            foreach (var course in entity.Courses)
                if (_repository.GetSingleById(course.DishId) == null)
                    result.AddError($"Dish with Id {course.DishId} not found.");

        return Task.FromResult(result);
    }
}
