using FluentAssertions;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Api.Exceptions;
using Mealmap.Api.InputMappers;
using Mealmap.Domain.DishAggregate;
using Mealmap.Domain.MealAggregate;
using Moq;

namespace Mealmap.Api.UnitTests;

public class MealInputHandlerTests
{
    private readonly MealInputHandler _mapper;
    private readonly Dish _dish;

    public MealInputHandlerTests()
    {
        _dish = new Dish("Krabby Patty");
        var _dishRepositoryMock = Mock.Of<IDishRepository>(m =>
            m.GetSingleById(_dish.Id) == _dish);

        _mapper = new MealInputHandler(
            new MealService(
                Mock.Of<IDishRepository>(m => m.GetSingleById(It.IsAny<Guid>()) == _dish)));
    }

    [Fact]
    public void FromDataTransferObject_WhenDiningDate_ReturnsWithDiningDate()
    {
        var diningDate = DateOnly.FromDateTime(DateTime.Now);
        var dto = new MealDTO() { DiningDate = diningDate };

        var meal = _mapper.FromDataTransferObject(dto);

        meal.DiningDate.Should().Be(diningDate);
    }

    [Fact]
    public void FromDataTransferObject_WhenSingleCourse_ReturnsWithThatCourse()
    {
        var existingDishId = _dish.Id;
        var courses = new CourseDTO[] { new CourseDTO() { Index = 1, DishId = existingDishId, MainCourse = true } };
        var dto = new MealDTO() { DiningDate = DateOnly.FromDateTime(DateTime.Now), Courses = courses };

        var meal = _mapper.FromDataTransferObject(dto);

        meal.Courses.Should().HaveCount(1);
        meal.Courses.First().DishId.Should().NotBeEmpty();
        meal.Courses.First().DishId.Should().Be(existingDishId);
    }

    [Fact]
    public void FromDataTransferObject_WhenMultipleCourses_ReturnsWithMultipleCourse()
    {
        var existingDishId = _dish.Id;
        var courses = new CourseDTO[] {
            new CourseDTO() { Index = 1, DishId = existingDishId, MainCourse = true },
            new CourseDTO() { Index = 2, DishId = existingDishId, MainCourse = false },
            new CourseDTO() { Index = 3, DishId = existingDishId, MainCourse = false }
        };
        var dto = new MealDTO() { DiningDate = DateOnly.FromDateTime(DateTime.Now), Courses = courses };

        var meal = _mapper.FromDataTransferObject(dto);

        meal.Courses.Should().HaveCount(3);
    }

    [Fact]
    public void FromDataTransferObject_WhenIdIsSet_ThrowsValidationException()
    {
        var someGuid = Guid.NewGuid();
        var someMealDate = DateOnly.FromDateTime(DateTime.Now);
        var dto = new MealDTO()
        {
            Id = someGuid,
            DiningDate = someMealDate,
        };

        Action act = () => _mapper.FromDataTransferObject(dto);

        act.Should().Throw<ValidationException>();
    }
}
