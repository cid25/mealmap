namespace Mealmap.Domain.MealAggregate;

#pragma warning disable CA1822
public class MealFactory
{
    public Meal CreateMealWith(DateOnly diningDate)
    {
        return new Meal(diningDate);
    }
    public Meal CreateMealWith(Guid id, DateOnly diningDate)
    {
        return new Meal(id, diningDate);
    }
}
