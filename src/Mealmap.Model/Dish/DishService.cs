namespace Mealmap.Domain.DishAggregate;

public class DishService
{
#pragma warning disable CA1822
    public Dish Create(Guid id, string name, string? description, int servings)
    {
        return new Dish(id, name) { Description = description, Servings = servings };
    }

    public Dish Create(string name, string? description, int servings)
    {
        return new Dish(name) { Description = description, Servings = servings };
    }

    public void ChangeName(Dish dish, string name)
    {
        dish.Name = name;
    }

    public void SetVersion(Dish dish, byte[] version)
    {
        dish.Version = version;
    }

    public void SetDescription(Dish dish, string? description)
    {
        dish.Description = description;
    }

    public void SetServings(Dish dish, int servings)
    {
        dish.Servings = servings;
    }


    public void SetImage(Dish dish, byte[] content, string mediaType)
    {
        dish.Image = new DishImage(content, mediaType);
    }

    public void RemoveImage(Dish dish)
    {
        dish.Image = null;
    }

    public void AddIngredient(Dish dish, decimal quantity, string unitOfMeasurementName, string description)
    {
        dish.AddIngredient(quantity, unitOfMeasurementName, description);
    }

    public void RemoveIngredient(Dish dish, decimal quantity, string unitOfMeasurementName, string description)
    {
        dish.RemoveIngredient(quantity, unitOfMeasurementName, description);
    }

    public void RemoveAllIngredients(Dish dish)
    {
        dish.RemoveAllIngredients();
    }
#pragma warning restore CA1822
}
