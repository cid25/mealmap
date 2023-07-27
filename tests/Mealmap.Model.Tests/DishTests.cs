using FluentAssertions;
using Mealmap.Domain.DishAggregate;

namespace Mealmap.Domain.Tests;

public class DishTests
{
    [Fact]
    public void AddIngredientToEmptySet_AddsIngredient()
    {
        Dish dish = new("Tuna Supreme");

        dish.AddIngredient(1, "Kilogram", "Sardine filets");

        dish.Ingredients.Should().HaveCount(1);
    }

    [Fact]
    public void AddIngredientToNonemptySet_AddsIngredient()
    {
        Dish dish = new("Tuna Supreme")
        {
            Ingredients = new List<Ingredient>() {
                new Ingredient(100, new UnitOfMeasurement("Mililiter"), "Milk")
            }
        };

        dish.AddIngredient(1, "Kilogram", "Sardine filets");

        dish.Ingredients.Should().HaveCount(2);
    }

    [Fact]
    public void RemoveIngredient_RemovesIngredient()
    {
        Dish dish = new("Tuna Supreme")
        {
            Ingredients = new List<Ingredient>() {
            new Ingredient(1, new UnitOfMeasurement("Kilogram"), "Sardine filets") }
        };

        dish.RemoveIngredient(1, "Kilogram", "Sardine filets");

        dish.Ingredients.Should().HaveCount(0);
    }
}
