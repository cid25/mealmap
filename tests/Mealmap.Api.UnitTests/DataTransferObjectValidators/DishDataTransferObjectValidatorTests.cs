﻿using Mealmap.Api.CommandValidators;
using Mealmap.Api.DataTransferObjects;

namespace Mealmap.Api.UnitTests.DataTransferObjectValidators;

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

    [Fact]
    public void Dish_is_invalid_when_unit_of_measurement_is_not_known()
    {
        DishDTO dish = new("dummy")
        {
            Ingredients = new[] {
                new IngredientDTO(1, "NonsenseUnit", "Fake"),
                new IngredientDTO(2, "BullshitUnit", "Stub")
            }
        };

        var sut = new DishDataTransferObjectValidator();

        var result = sut.Validate(dish);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
    }
}
