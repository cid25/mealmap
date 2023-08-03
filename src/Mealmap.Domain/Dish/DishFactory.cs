namespace Mealmap.Domain.DishAggregate;

#pragma warning disable CA1822
public class DishFactory
{
    public Dish CreateDishWith(string name, string? description, int servings)
    {
        return new Dish(name) { Description = description, Servings = servings };
    }

    public Dish CreateDishWith(Guid id, string name, string? description, int servings)
    {
        return new Dish(id, name) { Description = description, Servings = servings };
    }
}
