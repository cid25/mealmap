using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.DishAggregate;
using Mealmap.Domain.MealAggregate;

namespace Mealmap.Domain.UnitTests.MealAggregate;

public class MealValidatorTests
{
    private readonly Meal _meal;

    public MealValidatorTests()
    {
        _meal = new Meal(DateOnly.FromDateTime(DateTime.Now));
        _meal.AddCourse(index: 1, mainCourse: true, attendees: 1, dishId: Guid.NewGuid());
    }

    [Fact]
    public void ValidateAsync_WhenDishInCourseValid_ReturnsValidResult()
    {
        // Arrange
        var repository = Mock.Of<IRepository<Dish>>(m => m.GetSingleById(It.IsAny<Guid>()) == new Dish("dummyName"));
        var sut = new MealValidator(repository);

        // Act
        var result = sut.ValidateAsync(_meal);

        // Assert
        result.Result.IsValid.Should().BeTrue();
        result.Result.Errors.Should().HaveCount(0);
    }

    [Fact]
    public void ValidateAsync_WhenDishInCourseNotFound_ReturnsErrorResult()
    {
        // Arrange
        var repository = Mock.Of<IRepository<Dish>>(m => m.GetSingleById(It.IsAny<Guid>()) == null);
        var sut = new MealValidator(repository);

        // Act
        var result = sut.ValidateAsync(_meal);

        // Assert
        result.Result.IsValid.Should().BeFalse();
        result.Result.Errors.Should().HaveCount(1);
    }
}
