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

#pragma warning disable CA1822
    public Meal CreateMeal(Guid id, DateOnly diningDate)
    {
        return new Meal(id, diningDate);
    }

    public Meal CreateMeal(DateOnly diningDate)
    {
        return new Meal(diningDate);
    }
    public void SetVersion(Meal meal, byte[] version)
    {
        meal.Version = version;
    }

    public void ChangeDiningDate(Meal meal, DateOnly diningDate)
    {
        meal.ChangeDiningDate(diningDate);
    }

    /// <exception cref="DomainValidationException"></exception>
    public void AddCourseToMeal(Meal meal, int index, bool mainCourse, Guid dishId)
    {
        if (_dishRepository.GetSingleById(dishId) == null)
            throw new DomainValidationException($"There is no dish with id {dishId}.");

        meal.AddCourse(index, mainCourse, dishId);
    }

    public void RemoveAllCourses(Meal meal)
    {
        meal.RemoveAllCourses();
    }
#pragma warning restore CA1822
}
