using Mealmap.Domain.DishAggregate;
using Mealmap.Domain.MealAggregate;

namespace Mealmap.Domain.UnitTests.MealAggregate;

public class MealValidatorTests
{
    [Fact]
    public void ValidateAsync_WhenDishValid_ReturnsValidResult()
    {
        // Arrange
        var meal = new Meal(DateOnly.FromDateTime(DateTime.Now));
        meal.AddCourse(1, true, Guid.NewGuid());
        var repository = Mock.Of<IDishRepository>(m => m.GetSingleById(It.IsAny<Guid>()) == new Dish("fakeName"));
        var sut = new MealValidator(repository);

        // Act
        var result = sut.ValidateAsync(meal);

        // Assert
        result.Result.IsValid.Should().BeTrue();
        result.Result.Errors.Should().HaveCount(0);
    }

    [Fact]
    public void ValidateAsync_WhenDishNotFound_ReturnsErrorResult()
    {
        // Arrange
        var meal = new Meal(DateOnly.FromDateTime(DateTime.Now));
        meal.AddCourse(1, true, Guid.NewGuid());
        var repository = Mock.Of<IDishRepository>(m => m.GetSingleById(It.IsAny<Guid>()) == null);
        var sut = new MealValidator(repository);

        // Act
        var result = sut.ValidateAsync(meal);

        // Assert
        result.Result.IsValid.Should().BeFalse();
        result.Result.Errors.Should().HaveCount(1);
    }
}
