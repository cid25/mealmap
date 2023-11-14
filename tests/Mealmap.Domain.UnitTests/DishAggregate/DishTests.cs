using AutoFixture;
using Mealmap.Domain.DishAggregate;

namespace Mealmap.Domain.UnitTests.DishAggregate;

public class DishTests
{
    private readonly Dish _dish;

    public DishTests()
    {
        var fixture = new Fixture();
        _dish = fixture.Create<Dish>();
    }

    [Fact]
    public void AddIngredientToEmptySet_AddsIngredient()
    {
        _dish.AddIngredient(1, "Kilogram", "Sardine filets");

        _dish.Ingredients.Should().HaveCount(1);
    }

    [Fact]
    public void AddIngredientToNonemptySet_AddsIngredient()
    {
        _dish.AddIngredient(100, "Mililiter", "Milk");

        _dish.AddIngredient(1, "Kilogram", "Sardine filets");

        _dish.Ingredients.Should().HaveCount(2);
    }

    [Fact]
    public void RemoveIngredient_RemovesIngredient()
    {
        _dish.AddIngredient(100, "Mililiter", "Milk");

        _dish.RemoveIngredient(100, "Mililiter", "Milk");

        _dish.Ingredients.Should().HaveCount(0);
    }

    [Fact]
    public void RemoveIngredientType_RemovesIngredient()
    {
        _dish.AddIngredient(100, "Mililiter", "Milk");

        var ingredient = _dish.Ingredients.First();
        _dish.RemoveIngredient(ingredient);

        _dish.Ingredients.Should().HaveCount(0);
    }

    [Fact]
    public void RemoveAllIngredients_RemovesIngredient()
    {
        _dish.AddIngredient(100, "Mililiter", "Milk");
        _dish.AddIngredient(1, "Kilogram", "Sardine filets");

        _dish.RemoveAllIngredients();

        _dish.Ingredients.Should().HaveCount(0);
    }
}
