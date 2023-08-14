using Mealmap.Api.Behaviors;
using Mealmap.Api.Commands;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Domain.DishAggregate;
using MediatR;

namespace Mealmap.Api.UnitTests.Behaviors;

public class MealCommandValidationBehaviorTests
{
    [Fact]
    public async void Handle_WhenNoCourses_ReturnsNext()
    {
        // Arrange
        MealDTO dto = new();
        var behavior = new MealCommandValidationBehavior(Mock.Of<IDishRepository>());
        var next = new Mock<RequestHandlerDelegate<CommandNotification<MealDTO>>>();

        // Act
        var result = await behavior.Handle(
            new UpdateMealCommand(Guid.NewGuid(), "fakeVersion", dto),
            next.Object,
            new CancellationTokenSource().Token);

        // Assert
        next.Verify(_ => _(), Times.Once());
    }

    [Fact]
    public async void Handle_WhenDishesValid_ReturnsNext()
    {
        // Arrange
        MealDTO dto = new();
        var behavior = new MealCommandValidationBehavior(Mock.Of<IDishRepository>());
        var next = new Mock<RequestHandlerDelegate<CommandNotification<MealDTO>>>();

        // Act
        var result = await behavior.Handle(
            new UpdateMealCommand(Guid.NewGuid(), "fakeVersion", dto),
            next.Object,
            new CancellationTokenSource().Token);

        // Assert
        next.Verify(_ => _(), Times.Once());
    }

    [Fact]
    public async void Handle_WhenDishesInvalid_ReturnsInvalidErrors()
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
        var behavior = new MealCommandValidationBehavior(mockDishRepository);

        // Act
        var result = await behavior.Handle(
            new UpdateMealCommand(Guid.NewGuid(), "fakeVersion", dto),
            Mock.Of<RequestHandlerDelegate<CommandNotification<MealDTO>>>(),
            new CancellationTokenSource().Token);

        // Assert
        result.Errors.Should().HaveCount(2);
        result.Errors[0].ErrorCode.Should().Be(CommandErrorCodes.NotValid);
    }
}
