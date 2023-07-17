using FluentAssertions;

namespace Mealmap.Model.Tests
{
    public class DishTests
    {
        [Fact]
        public void AddIngredient_AddsIngredient()
        {
            Dish dish = new("Tuna Supreme");

            dish.AddIngredient(1, "Kilogram", "Sardine filets");

            dish.Ingredients.Should().HaveCount(1);
        }

        [Fact]
        public void RemoveIngredient_RemovesIngredient()
        {
            Dish dish = new("Tuna Supreme") { Ingredients = new List<Ingredient>() { new Ingredient(1, "Kilogram", "Sardine filets") } };

            dish.RemoveIngredient(1, "Kilogram", "Sardine filets");

            dish.Ingredients.Should().HaveCount(0);
        }
    }
}
