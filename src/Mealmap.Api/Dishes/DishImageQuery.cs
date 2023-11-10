namespace Mealmap.Api.Dishes;

public class DishImageQuery
{
    public Guid Id { get; init; }

    public DishImageQuery(Guid id)
    {
        Id = id;
    }
}
