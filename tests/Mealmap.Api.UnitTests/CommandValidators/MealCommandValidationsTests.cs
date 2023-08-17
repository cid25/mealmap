using Mealmap.Api.CommandValidators;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Domain.DishAggregate;

namespace Mealmap.Api.UnitTests.CommandValidators;

public class MealCommandValidationsTests
{
    [Fact]
    public void ValidateSingleMainCourseOnly_WhenOnlySingleMainCourse_ReturnsEmptyList()
    {
        // Arrange
        MealDTO dto = new()
        {
            Courses = new CourseDTO[3] {
                new CourseDTO() { Index = 1, MainCourse = true, DishId = Guid.NewGuid() },
                new CourseDTO() { Index = 2, MainCourse = false, DishId = Guid.NewGuid() },
                new CourseDTO() { Index = 3, MainCourse = false, DishId = Guid.NewGuid() },
            }
        };

        var validations = new MealCommandValidations(Mock.Of<IDishRepository>());

        // Act
        var result = validations.ValidateSingleMainCourseOnly(dto);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ValidateSingleMainCourseOnly_WhenOnlySingleMainCourse_ReturnsNotValidError()
    {
        // Arrange
        MealDTO dto = new()
        {
            Courses = new CourseDTO[3] {
                new CourseDTO() { Index = 1, MainCourse = true, DishId = Guid.NewGuid() },
                new CourseDTO() { Index = 2, MainCourse = true, DishId = Guid.NewGuid() },
                new CourseDTO() { Index = 3, MainCourse = false, DishId = Guid.NewGuid() },
            }
        };

        var validations = new MealCommandValidations(Mock.Of<IDishRepository>());

        // Act
        var result = validations.ValidateSingleMainCourseOnly(dto);

        // Assert
        result!.ErrorCode.Should().Be(CommandErrorCodes.NotValid);
    }

    [Fact]
    public void ValidateDishesExist_WhenDishesDoExist_ReturnsEmptyList()
    {
        // Arrange
        MealDTO dto = new()
        {
            Courses = new CourseDTO[2] {
                new CourseDTO() { Index = 1, MainCourse = true, DishId = Guid.NewGuid() },
                new CourseDTO() { Index = 2, MainCourse = false, DishId = Guid.NewGuid() },
            }
        };
        var mockDishRepository = Mock.Of<IDishRepository>(m => m.GetSingleById(It.IsAny<Guid>())
            == new Dish("fakeName"));
        var validations = new MealCommandValidations(mockDishRepository);

        // Act
        var result = validations.ValidateDishesExist(dto);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void ValidateDishesExist_WhenDishesDoNotExist_ReturnsNotValidErrors()
    {
        // Arrange
        MealDTO dto = new()
        {
            Courses = new CourseDTO[2] {
                new CourseDTO() { Index = 1, MainCourse = true, DishId = Guid.NewGuid() },
                new CourseDTO() { Index = 2, MainCourse = false, DishId = Guid.NewGuid() },
            }
        };
        var mockDishRepository = Mock.Of<IDishRepository>(m => m.GetSingleById(It.IsAny<Guid>()) == null);
        var validations = new MealCommandValidations(mockDishRepository);

        // Act
        var result = validations.ValidateDishesExist(dto);

        // Assert
        result.Should().HaveCount(2);
        result.First().ErrorCode.Should().Be(CommandErrorCodes.NotValid);
    }
}
