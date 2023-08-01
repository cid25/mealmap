namespace Mealmap.Domain.MealAggregate
{
    public interface IMealService
    {
        void AddCourseToMeal(Meal meal, int index, bool mainCourse, Guid dishId);
        public Meal CreateMeal(DateOnly diningDate);
        public Meal CreateMeal(Guid id, DateOnly diningDate);

        public void SetVersion(Meal meal, byte[] version);

        public void ChangeDiningDate(Meal meal, DateOnly diningDate);

        public void RemoveAllCourses(Meal meal);
    }
}
