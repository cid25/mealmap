using System.ComponentModel.DataAnnotations;

namespace Mealmap.Domain.DishAggregate;

public class Dish
{
    public Guid Id { get; }

    public byte[]? Version { get; internal set; }

    [MaxLength(100)]
    public string Name { get; internal set; }

    public string? Description { get; internal set; }

    [Range(1, int.MaxValue)]
    public int Servings { get; internal set; }

    public DishImage? Image { get; internal set; }

    private List<Ingredient> _ingredients;

    public IEnumerable<Ingredient> Ingredients
    {
        get => _ingredients;
    }

    internal Dish(Guid id, string name)
    {
        Id = id;
        Name = name;
        _ingredients = new List<Ingredient>();
    }

    internal Dish(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
        _ingredients = new List<Ingredient>();
    }

    /// <exception cref="DomainValidationException"></exception>
    internal void AddIngredient(decimal quantity, string unitOfMeasurementName, string description)
    {
        var unit = new UnitOfMeasurement(unitOfMeasurementName);
        Ingredient ingredient = new(quantity, unit, description);

        _ingredients.Add(ingredient);
    }

    internal void ReplaceIngredientsWith(ICollection<Ingredient> ingredients)
    {
        _ingredients = ingredients.ToList();
    }

    /// <exception cref="DomainValidationException"></exception>
    internal void RemoveIngredient(decimal quantity, string unitOfMeasurementName, string description)
    {
        if (!Ingredients.Any())
            return;

        var unit = new UnitOfMeasurement(unitOfMeasurementName);
        Ingredient newIngredient = new(quantity, unit, description);

        _ingredients.Remove(newIngredient);
    }

    internal void RemoveIngredient(Ingredient ingredient)
    {
        _ingredients.Remove(ingredient);
    }

    internal void RemoveAllIngredients()
    {
        _ingredients = new List<Ingredient>();
    }
}
