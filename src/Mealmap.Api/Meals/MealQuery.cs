namespace Mealmap.Api.Meals;

public class MealQuery(Guid id)
{
    public Guid Id { get; init; } = id;
}
