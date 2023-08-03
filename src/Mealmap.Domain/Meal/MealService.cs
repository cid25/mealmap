using Mealmap.Domain.Common;
using Mealmap.Domain.DishAggregate;

namespace Mealmap.Domain.MealAggregate;

public class MealService : IMealService
{
    private readonly IDishRepository _dishRepository;

    public MealService(IDishRepository dishRepository)
    {
        _dishRepository = dishRepository;
    }

    /// <exception cref="DomainValidationException"></exception>
    public void AddCourseToMeal(Meal meal, int index, bool mainCourse, Guid dishId)
    {
        if (_dishRepository.GetSingleById(dishId) == null)
            throw new DomainValidationException($"There is no dish with id {dishId}.");

        meal.AddCourse(index, mainCourse, dishId);
    }
}
