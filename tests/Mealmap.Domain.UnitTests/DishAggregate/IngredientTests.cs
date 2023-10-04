using Mealmap.Domain.Common.Validation;
using Mealmap.Domain.DishAggregate;

namespace Mealmap.Domain.UnitTests.DishAggregate;

public class IngredientTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WhenQuantityZeroOrLower_ThrowsDomainValidationException(decimal quantity)
    {
        var act = () => new Ingredient(quantity, "Gram", "fakeDescription");

        act.Should().Throw<DomainValidationException>();
    }
}
