using Mealmap.Domain.DishAggregate;

namespace Mealmap.Domain.UnitTests.DishAggregate;

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
        Dish dish = new("Tuna Supreme");
        dish.AddIngredient(100, "Mililiter", "Milk");

        dish.AddIngredient(1, "Kilogram", "Sardine filets");

        dish.Ingredients.Should().HaveCount(2);
    }

    [Fact]
    public void RemoveIngredient_RemovesIngredient()
    {
        Dish dish = new("Tuna Supreme");
        dish.AddIngredient(100, "Mililiter", "Milk");

        dish.RemoveIngredient(100, "Mililiter", "Milk");

        dish.Ingredients.Should().HaveCount(0);
    }

    [Fact]
    public void RemoveIngredientType_RemovesIngredient()
    {
        Dish dish = new("Tuna Supreme");
        dish.AddIngredient(100, "Mililiter", "Milk");

        var ingredient = dish.Ingredients.First();
        dish.RemoveIngredient(ingredient);

        dish.Ingredients.Should().HaveCount(0);
    }

    [Fact]
    public void RemoveAllIngredients_RemovesIngredient()
    {
        Dish dish = new("Tuna Supreme");
        dish.AddIngredient(100, "Mililiter", "Milk");
        dish.AddIngredient(1, "Kilogram", "Sardine filets");

        dish.RemoveAllIngredients();

        dish.Ingredients.Should().HaveCount(0);
    }
}
