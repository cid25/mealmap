namespace Mealmap.Domain.MealAggregate
{
    public interface IMealService
    {
        void AddCourseToMeal(Meal meal, int index, bool mainCourse, Guid dishId);
        Meal CreateMeal(DateOnly diningDate);
        Meal CreateMeal(Guid id, DateOnly diningDate);
    }
}
