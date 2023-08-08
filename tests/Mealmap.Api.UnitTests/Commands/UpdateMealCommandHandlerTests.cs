using FluentAssertions;
using Mealmap.Api.Commands;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Api.OutputMappers;
using Mealmap.Domain.MealAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Mealmap.Api.UnitTests.Commands;

public class UpdateMealCommandHandlerTests
{

    [Fact]
    public async void Handle_SavesUpdateAndReturnsDTO()
    {
        // Arrange
        var aGuid = Guid.NewGuid();
        const string aVersion = "AAAAAAAA";
        MealDTO dto = new();

        Meal dummyMeal = new(aGuid, DateOnly.FromDateTime(DateTime.Now));

        var mockRepository = new Mock<IMealRepository>();
        mockRepository.Setup(m => m.GetSingleById(It.Is<Guid>(g => g == aGuid))).Returns(dummyMeal);
        var handler = new UpdateMealCommandHandler(
            mockRepository.Object,
            Mock.Of<IOutputMapper<MealDTO, Meal>>(m => m.FromEntity(dummyMeal) == dto),
            Mock.Of<ILogger<UpdateMealCommandHandler>>()
        );

        // Act
        var result = await handler.Handle(
            new UpdateMealCommand(aGuid, aVersion, dto),
            new CancellationTokenSource().Token); ;

        // Assert
        mockRepository.Verify(m => m.Update(It.Is<Meal>(m => m == dummyMeal)), Times.Once);
        result.Result.Should().NotBeNull();
    }

    [Fact]
    public async void Handle_WhenMealDoesNotExist_ReturnsNotificationWithNotFoundError()
    {
        // Arrange
        var aGuid = Guid.NewGuid();
        const string aVersion = "AAAAAAAA";
        MealDTO dto = new();

        var mockRepository = new Mock<IMealRepository>();
        mockRepository.Setup(m => m.GetSingleById(It.Is<Guid>(g => g == aGuid))).Returns(value: null);
        var handler = new UpdateMealCommandHandler(
            mockRepository.Object,
            Mock.Of<IOutputMapper<MealDTO, Meal>>(),
            Mock.Of<ILogger<UpdateMealCommandHandler>>()
        );

        // Act
        var result = await handler.Handle(
            new UpdateMealCommand(aGuid, aVersion, dto),
            new CancellationTokenSource().Token);

        // Assert
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorCode.Should().Be(CommandErrorCodes.NotFound);
    }

    [Fact]
    public async void Handle_WhenSavingThrowsConcurrencyException_ReturnsNotificationWithEtagMismatchError()
    {
        // Arrange
        var aGuid = Guid.NewGuid();
        const string aVersion = "AAAAAAAA";
        MealDTO dto = new();

        Meal dummyMeal = new(aGuid, DateOnly.FromDateTime(DateTime.Now));

        var mockRepository = new Mock<IMealRepository>();
        mockRepository.Setup(m => m.GetSingleById(It.IsAny<Guid>())).Returns(dummyMeal);
        mockRepository.Setup(m => m.Update(It.IsAny<Meal>())).Throws(new DbUpdateConcurrencyException());
        var handler = new UpdateMealCommandHandler(
            mockRepository.Object,
            Mock.Of<IOutputMapper<MealDTO, Meal>>(),
            Mock.Of<ILogger<UpdateMealCommandHandler>>()
        );

        // Act
        var result = await handler.Handle(
            new UpdateMealCommand(aGuid, aVersion, dto),
            new CancellationTokenSource().Token);

        // Assert
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorCode.Should().Be(CommandErrorCodes.EtagMismatch);
    }
}
