using FluentAssertions;
using Mealmap.Api.Commands;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Api.OutputMappers;
using Mealmap.Domain.DishAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Mealmap.Api.UnitTests.Commands;

public class UpdateDishCommandHandlerTests
{

    [Fact]
    public async void Handle_SavesUpdateAndReturnsDTO()
    {
        // Arrange
        const string someDishName = "Sailors Surprise";
        const string aVersion = "AAAA";
        var aGuid = Guid.NewGuid();
        DishDTO dish = new(someDishName) { Id = aGuid };

        Dish dummyDish = new(aGuid, someDishName, null, 2);

        var mockRepository = new Mock<IDishRepository>();
        mockRepository.Setup(m => m.GetSingleById(It.IsAny<Guid>())).Returns(dummyDish);
        var handler = new UpdateDishCommandHandler(
            mockRepository.Object,
            Mock.Of<IOutputMapper<DishDTO, Dish>>(m => m.FromEntity(dummyDish) == dish),
            Mock.Of<ILogger<UpdateDishCommandHandler>>()
        );

        // Act
        var result = await handler.Handle(
            new UpdateDishCommand(aGuid, aVersion, dish),
            new CancellationTokenSource().Token);

        // Assert
        mockRepository.Verify(m => m.Update(It.IsAny<Dish>()), Times.Once);
        result.Result.Should().NotBeNull();
    }

    [Fact]
    public async void Handle_WhenDishDoesNotExist_ReturnsNotificationWithNotFoundError()
    {
        // Arrange
        const string someDishName = "Sailors Surprise";
        const string aVersion = "AAAA";
        var aGuid = Guid.NewGuid();
        DishDTO dish = new(someDishName) { Id = aGuid };

        var mockRepository = new Mock<IDishRepository>();
        mockRepository.Setup(m => m.GetSingleById(It.IsAny<Guid>())).Returns(value: null);
        var handler = new UpdateDishCommandHandler(
            mockRepository.Object,
            Mock.Of<IOutputMapper<DishDTO, Dish>>(),
            Mock.Of<ILogger<UpdateDishCommandHandler>>()
        );

        // Act
        var result = await handler.Handle(
            new UpdateDishCommand(aGuid, aVersion, dish),
            new CancellationTokenSource().Token);

        // Assert
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorCode.Should().Be(CommandErrorCodes.NotFound);
    }

    [Fact]
    public async void Handle_WhenSavingThrowsConcurrencyException_ReturnsNotificationWithEtagMismatchError()
    {
        // Arrange
        const string someDishName = "Sailors Surprise";
        const string aVersion = "AAAA";
        var aGuid = Guid.NewGuid();
        DishDTO dish = new(someDishName) { Id = aGuid };

        Dish dummyDish = new(aGuid, someDishName, null, 2);

        var mockRepository = new Mock<IDishRepository>();
        mockRepository.Setup(m => m.GetSingleById(It.IsAny<Guid>())).Returns(dummyDish);
        mockRepository.Setup(m => m.Update(It.IsAny<Dish>())).Throws(new DbUpdateConcurrencyException());
        var handler = new UpdateDishCommandHandler(
            mockRepository.Object,
            Mock.Of<IOutputMapper<DishDTO, Dish>>(),
            Mock.Of<ILogger<UpdateDishCommandHandler>>()
        );

        // Act
        var result = await handler.Handle(
            new UpdateDishCommand(aGuid, aVersion, dish),
            new CancellationTokenSource().Token);

        // Assert
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorCode.Should().Be(CommandErrorCodes.EtagMismatch);
    }

}
