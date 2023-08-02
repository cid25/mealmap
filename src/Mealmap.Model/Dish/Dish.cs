﻿using System.ComponentModel.DataAnnotations;
using Mealmap.Domain.Common;

namespace Mealmap.Domain.DishAggregate;

public class Dish : EntityBase
{
    [MaxLength(100)]
    public string Name { get; set; }

    public string? Description { get; set; }

    [Range(1, int.MaxValue)]
    public int Servings { get; set; }

    public DishImage? Image { get; private set; }

    private List<Ingredient> _ingredients;

    public IEnumerable<Ingredient> Ingredients
    {
        get => _ingredients;
    }
    internal Dish(string name) : base()
    {
        Name = name;
        _ingredients = new List<Ingredient>();
    }

    internal Dish(Guid id, string name) : base(id)
    {
        Name = name;
        _ingredients = new List<Ingredient>();
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
        var unit = new UnitOfMeasurement(unitOfMeasurementName);
        Ingredient ingredient = new(quantity, unit, description);

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
