using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.Common.Validation;
using Mealmap.Domain.DishAggregate;

namespace Mealmap.Domain.MealAggregate;

public class MealValidator(IRepository<Dish> repository) : AbstractEntityValidator<Meal>
{
    public override Task<DomainValidationResult> ValidateAsync(Meal entity)
    {
        DomainValidationResult result = new();

        if (entity.Courses.Count != 0)
            foreach (var course in entity.Courses)
                if (repository.GetSingleById(course.DishId) == null)
                    result.AddError($"Dish with Id {course.DishId} not found.");

        return Task.FromResult(result);
    }
}
