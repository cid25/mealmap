using Mealmap.Api.Dishes;

namespace Mealmap.Api.UnitTests.Dishes;

public class DishDataTransferObjectValidatorTests
{
    [Fact]
    public void Dish_is_valid_when_quantities_greater_than_zero()
    {
        DishDTO dish = new("dummy")
        {
            Ingredients = new[] {
                new IngredientDTO(1, "Gram", "Fake"),
                new IngredientDTO(2, "Kilogram", "Stub")
            }
        };

        var sut = new DishDataTransferObjectValidator();

        var result = sut.Validate(dish);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Dish_is_invalid_when_quantities_less_than_or_equal_to_zero()
    {
        DishDTO dish = new("dummy")
        {
            Ingredients = new[] {
                new IngredientDTO(0, "Gram", "Fake"),
                new IngredientDTO(-1, "Kilogram", "Stub")
            }
        };

        var sut = new DishDataTransferObjectValidator();

        var result = sut.Validate(dish);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
    }
}
