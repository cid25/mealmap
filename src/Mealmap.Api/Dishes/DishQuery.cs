namespace Mealmap.Api.Dishes;

public class DishQuery
{
    public Guid Id { get; init; }

    public DishQuery(Guid id)
        => Id = id;
}
