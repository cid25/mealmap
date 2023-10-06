﻿using Mealmap.Api.CommandHandlers;
using Mealmap.Api.Commands;
using Mealmap.Api.CommandValidators;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Api.OutputMappers;
using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.Common.Validation;
using Mealmap.Domain.DishAggregate;
using Microsoft.Extensions.Logging;

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

        var mockRepository = new Mock<IRepository<Dish>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        mockRepository.Setup(m => m.GetSingleById(It.Is<Guid>(g => g == aGuid))).Returns(dummyDish);
        var handler = new UpdateDishCommandHandler(
            mockRepository.Object,
            mockUnitOfWork.Object,
            Mock.Of<IOutputMapper<DishDTO, Dish>>(m => m.FromEntity(dummyDish) == dto),
            Mock.Of<ILogger<UpdateDishCommandHandler>>(),
            new DishDataTransferObjectValidator()
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

        var mockRepository = new Mock<IRepository<Dish>>();
        mockRepository.Setup(m => m.GetSingleById(It.IsAny<Guid>())).Returns(value: null);
        var handler = new UpdateDishCommandHandler(
            mockRepository.Object,
            Mock.Of<IUnitOfWork>(),
            Mock.Of<IOutputMapper<DishDTO, Dish>>(),
            Mock.Of<ILogger<UpdateDishCommandHandler>>(),
            new DishDataTransferObjectValidator()
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
    public async void Handle_WhenSavingThrowsConcurrentUpdateException_ReturnsNotificationWithEtagMismatchError()
    {
        // Arrange
        var aGuid = Guid.NewGuid();
        const string aVersion = "AAAAAAAA";
        DishDTO dto = new("fakeDishName");

        Dish dummyDish = new(aGuid, "fakeDishName", null, 2);

        var mockRepository = new Mock<IRepository<Dish>>();
        mockRepository.Setup(m => m.GetSingleById(It.IsAny<Guid>())).Returns(dummyDish);
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        mockUnitOfWork.Setup(m => m.SaveTransactionAsync()).Throws(new ConcurrentUpdateException());
        var handler = new UpdateDishCommandHandler(
            mockRepository.Object,
            mockUnitOfWork.Object,
            Mock.Of<IOutputMapper<DishDTO, Dish>>(),
            Mock.Of<ILogger<UpdateDishCommandHandler>>(),
            new DishDataTransferObjectValidator()
        );

        // Act
        var result = await handler.Handle(
            new UpdateDishCommand(aGuid, aVersion, dto),
            new CancellationTokenSource().Token);

        // Assert
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorCode.Should().Be(CommandErrorCodes.VersionMismatch);
    }

    [Fact]
    public async void Handle_WhenSavingThrowsDomainValidationException_ReturnsNotificationWithNotValidError()
    {
        // Arrange
        var aGuid = Guid.NewGuid();
        const string aVersion = "AAAAAAAA";
        DishDTO dto = new("fakeDishName");

        Dish dummyDish = new(aGuid, "fakeDishName", null, 2);

        var mockRepository = new Mock<IRepository<Dish>>();
        mockRepository.Setup(m => m.GetSingleById(It.IsAny<Guid>())).Returns(dummyDish);
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        mockUnitOfWork.Setup(m => m.SaveTransactionAsync()).Throws(new DomainValidationException(String.Empty));
        var handler = new UpdateDishCommandHandler(
            mockRepository.Object,
            mockUnitOfWork.Object,
            Mock.Of<IOutputMapper<DishDTO, Dish>>(),
            Mock.Of<ILogger<UpdateDishCommandHandler>>(),
            new DishDataTransferObjectValidator()
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
