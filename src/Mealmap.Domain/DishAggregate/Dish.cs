using System.ComponentModel.DataAnnotations;
using Mealmap.Domain.Common;
using Mealmap.Domain.Common.Validation;

namespace Mealmap.Domain.DishAggregate;

public class Dish : EntityBase
{
    private List<Ingredient> _ingredients = new();

    [MaxLength(50)]
    public string Name { get; set; }

    [MaxLength(80)]
    public string? Description { get; set; }

    [Range(1, int.MaxValue)]
    public int Servings { get; set; }

    public DishImage? Image { get; private set; }

    public string? Instructions { get; set; }

    public IReadOnlyCollection<Ingredient> Ingredients
    {
        get => _ingredients.AsReadOnly();
    }
    public Dish(string name) : base()
    {
        Name = name;
    }

    public Dish(Guid id, string name) : base(id)
    {
        Name = name;
    }

    public Dish(string name, string? description, int servings)
    {
        (Name, Description, Servings) = (name, description, servings);
    }

    public Dish(Guid id, string name, string? description, int servings) : base(id)
    {
        (Name, Description, Servings) = (name, description, servings);
    }

    public void SetImage(byte[] content, string mediaType)
    {
        Image = new DishImage(content, mediaType);
    }

    public void RemoveImage()
    {
        Image = null;
    }

    /// <exception cref="DomainValidationException"></exception>
    public void AddIngredient(decimal quantity, string unitOfMeasurementName, string description)
    {
        Ingredient ingredient = new(quantity, unitOfMeasurementName, description);

        _ingredients.Add(ingredient);
    }

    public void ReplaceIngredientsWith(ICollection<Ingredient> ingredients)
    {
        _ingredients = ingredients.ToList();
    }

    /// <exception cref="DomainValidationException"></exception>
    public void RemoveIngredient(decimal quantity, string unitOfMeasurementName, string description)
    {
        if (!Ingredients.Any())
            return;

        var unit = new UnitOfMeasurement(unitOfMeasurementName);
        Ingredient newIngredient = new(quantity, unit, description);

        _ingredients.Remove(newIngredient);
    }

    public void RemoveIngredient(Ingredient ingredient)
    {
        _ingredients.Remove(ingredient);
    }

    public void RemoveAllIngredients()
    {
        _ingredients = new List<Ingredient>();
    }
}
