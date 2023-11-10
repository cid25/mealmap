using Mealmap.Api.Meals;
using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.DishAggregate;

namespace Mealmap.Api.UnitTests.Meals;

public class MealDataTransferObjectValidatorTests
{
    [Fact]
    public void Meal_is_valid_when_dishes_exist()
    {
        // Arrange
        var guid = Guid.NewGuid();
        MealDTO dto = new()
        {
            Courses = new CourseDTO[2] {
                new CourseDTO() { Index = 1, MainCourse = true, DishId = guid },
                new CourseDTO() { Index = 2, MainCourse = false, DishId = guid },
            }
        };
        var sut = new MealDataTransferObjectValidator(Mock.Of<IRepository<Dish>>(m =>
            m.GetSingleById(guid) == new Dish("dummy")));

        // Act
        var result = sut.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Meal_is_invalid_when_dishes_do_not_exist()
    {
        // Arrange
        MealDTO dto = new()
        {
            Courses = new CourseDTO[2] {
                new CourseDTO() { Index = 1, MainCourse = true, DishId = Guid.NewGuid() },
                new CourseDTO() { Index = 2, MainCourse = false, DishId = Guid.NewGuid() },
            }
        };
        var sut = new MealDataTransferObjectValidator(Mock.Of<IRepository<Dish>>(m =>
            m.GetSingleById(It.IsAny<Guid>()) == null));

        // Act
        var result = sut.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
    }

    [Fact]
    public void Meal_is_valid_when_only_single_main_course()
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

        var sut = new MealDataTransferObjectValidator(Mock.Of<IRepository<Dish>>(m =>
            m.GetSingleById(It.IsAny<Guid>()) == new Dish("dummy")));

        // Act
        var result = sut.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Meal_is_invalid_when_multiple_main_courses()
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

        var sut = new MealDataTransferObjectValidator(Mock.Of<IRepository<Dish>>(m =>
            m.GetSingleById(It.IsAny<Guid>()) == new Dish("dummy")));

        // Act
        var result = sut.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
    }
}
