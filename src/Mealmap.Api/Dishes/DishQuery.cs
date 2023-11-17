namespace Mealmap.Api.Dishes;

public class DishQuery(Guid id)
{
    public Guid Id { get; init; } = id;
}
