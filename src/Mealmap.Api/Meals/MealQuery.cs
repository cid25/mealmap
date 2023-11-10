namespace Mealmap.Api.Meals;

public class MealQuery
{
    public Guid Id { get; init; }

    public MealQuery(Guid id)
    {
        Id = id;
    }
}
