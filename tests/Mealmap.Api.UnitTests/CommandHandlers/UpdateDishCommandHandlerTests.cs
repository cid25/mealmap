using FluentAssertions;
using Mealmap.Api.CommandHandlers;
using Mealmap.Api.Commands;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Api.OutputMappers;
using Mealmap.Domain;
using Mealmap.Domain.DishAggregate;
using Mealmap.Domain.Seedwork.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Mealmap.Api.UnitTests.CommandHandlers;

public class UpdateDishCommandHandlerTests
{

    [Fact]
    public async void Handle_SavesUpdateAndReturnsDTO()
    {
        // Arrange
        var aGuid = Guid.NewGuid();
        const string aVersion = "AAAAAAAA";
        DishDTO dto = new("fakeDishName");

        Dish dummyDish = new(aGuid, "fakeDishName", null, 2);

        var mockRepository = new Mock<IDishRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        mockRepository.Setup(m => m.GetSingleById(It.Is<Guid>(g => g == aGuid))).Returns(dummyDish);
        var handler = new UpdateDishCommandHandler(
            mockRepository.Object,
            mockUnitOfWork.Object,
            Mock.Of<IOutputMapper<DishDTO, Dish>>(m => m.FromEntity(dummyDish) == dto),
            Mock.Of<ILogger<UpdateDishCommandHandler>>()
        );

        // Act
        var result = await handler.Handle(
            new UpdateDishCommand(aGuid, aVersion, dto),
            new CancellationTokenSource().Token);

        // Assert
        mockRepository.Verify(m => m.Update(It.IsAny<Dish>()), Times.Once);
        mockUnitOfWork.Verify(m => m.SaveTransactionAsync(), Times.Once);
        result.Result.Should().NotBeNull();
    }

    [Fact]
    public async void Handle_WhenDishDoesNotExist_ReturnsNotificationWithNotFoundError()
    {
        // Arrange
        var aGuid = Guid.NewGuid();
        const string aVersion = "AAAAAAAA";
        DishDTO dto = new("fakeDishName");

        var mockRepository = new Mock<IDishRepository>();
        mockRepository.Setup(m => m.GetSingleById(It.IsAny<Guid>())).Returns(value: null);
        var handler = new UpdateDishCommandHandler(
            mockRepository.Object,
            Mock.Of<IUnitOfWork>(),
            Mock.Of<IOutputMapper<DishDTO, Dish>>(),
            Mock.Of<ILogger<UpdateDishCommandHandler>>()
        );

        // Act
        var result = await handler.Handle(
            new UpdateDishCommand(aGuid, aVersion, dto),
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
        DishDTO dto = new("fakeDishName");

        Dish dummyDish = new(aGuid, "fakeDishName", null, 2);

        var mockRepository = new Mock<IDishRepository>();
        mockRepository.Setup(m => m.GetSingleById(It.IsAny<Guid>())).Returns(dummyDish);
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        mockUnitOfWork.Setup(m => m.SaveTransactionAsync()).Throws(new DbUpdateConcurrencyException());
        var handler = new UpdateDishCommandHandler(
            mockRepository.Object,
            mockUnitOfWork.Object,
            Mock.Of<IOutputMapper<DishDTO, Dish>>(),
            Mock.Of<ILogger<UpdateDishCommandHandler>>()
        );

        // Act
        var result = await handler.Handle(
            new UpdateDishCommand(aGuid, aVersion, dto),
            new CancellationTokenSource().Token);

        // Assert
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorCode.Should().Be(CommandErrorCodes.EtagMismatch);
    }

    [Fact]
    public async void Handle_WhenSavingThrowsDomainValidationException_ReturnsNotificationWithNotValidError()
    {
        // Arrange
        var aGuid = Guid.NewGuid();
        const string aVersion = "AAAAAAAA";
        DishDTO dto = new("fakeDishName");

        Dish dummyDish = new(aGuid, "fakeDishName", null, 2);

        var mockRepository = new Mock<IDishRepository>();
        mockRepository.Setup(m => m.GetSingleById(It.IsAny<Guid>())).Returns(dummyDish);
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        mockUnitOfWork.Setup(m => m.SaveTransactionAsync()).Throws(new DomainValidationException(String.Empty));
        var handler = new UpdateDishCommandHandler(
            mockRepository.Object,
            mockUnitOfWork.Object,
            Mock.Of<IOutputMapper<DishDTO, Dish>>(),
            Mock.Of<ILogger<UpdateDishCommandHandler>>()
        );

        // Act
        var result = await handler.Handle(
            new UpdateDishCommand(aGuid, aVersion, dto),
            new CancellationTokenSource().Token);

        // Assert
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorCode.Should().Be(CommandErrorCodes.NotValid);
    }
}
