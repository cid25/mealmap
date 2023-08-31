using Mealmap.Domain.DishAggregate;

namespace Mealmap.Domain.UnitTests.DishAggregate;

public class IngredientTests
{
    [Fact]
    public void Equals_WhenObjectsOfSameValue_ReturnsTrue()
    {
        decimal quantity = 1m;
        UnitOfMeasurement unit = new("Gram");
        string description = "fakeDescription";
        var sut = new Ingredient(quantity, unit, description);
        var other = new Ingredient(quantity, unit, description);

        var result = sut.Equals(other);

        result.Should().BeTrue();
    }

    [Fact]
    public void Equals_WhenTypeNotMatching_ReturnsFalse()
    {
        decimal quantity = 1m;
        UnitOfMeasurement unit = new("Gram");
        string description = "fakeDescription";
        var sut = new Ingredient(quantity, unit, description);
        var other = new object();

        var result = sut.Equals(other);

        result.Should().BeFalse();
    }
}
