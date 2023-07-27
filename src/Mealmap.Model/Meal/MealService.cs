using Mealmap.Domain.DishAggregate;
using Mealmap.Domain.Exceptions;

namespace Mealmap.Domain.MealAggregate;

public class MealService
{
    private readonly IDishRepository _dishRepository;

    public MealService(IDishRepository dishRepository)
    {
        _dishRepository = dishRepository;
    }

#pragma warning disable CA1822
    public Meal CreateMeal(Guid id, DateOnly diningDate)
    {
        return new Meal(id, diningDate);
    }

    public Meal CreateMeal(DateOnly diningDate)
    {
        return new Meal(diningDate);
    }
#pragma warning restore CA1822

    public void AddCourseToMeal(Meal meal, int index, bool mainCourse, Guid dishId)
    {
        if (_dishRepository.GetSingleById(dishId) == null)
            throw new DomainValidationException($"There is no dish with id {dishId}.");

        meal.AddCourse(index, mainCourse, dishId);
    }
}
