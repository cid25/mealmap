using FluentAssertions;
using Mealmap.Domain.DishAggregate;
using Mealmap.Domain.Exceptions;
using Mealmap.Domain.MealAggregate;
using Moq;

namespace Mealmap.Domain.Tests;

public class MealServiceTests
{

    [Fact]
    public void AddCourseToMeal_WhenDishDoesntExist_ThrowsException()
    {
        MealService service = new(
            Mock.Of<IDishRepository>(m => m.GetSingleById(It.IsAny<Guid>()) == null));
        Meal aMeal = new(DateOnly.FromDateTime(DateTime.Now));

        Action act = () => service.AddCourseToMeal(aMeal, 1, false, Guid.NewGuid());

        act.Should().Throw<DomainValidationException>();
    }
}
