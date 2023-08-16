using Mealmap.Api.CommandHandlers;
using Mealmap.Api.Commands;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Api.OutputMappers;
using Mealmap.Domain.MealAggregate;
using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.Common.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Mealmap.Api.UnitTests.CommandHandlers;

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
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        mockRepository.Setup(m => m.GetSingleById(It.Is<Guid>(g => g == aGuid))).Returns(dummyMeal);
        var handler = new UpdateMealCommandHandler(
            mockRepository.Object,
            mockUnitOfWork.Object,
            Mock.Of<IOutputMapper<MealDTO, Meal>>(m => m.FromEntity(dummyMeal) == dto),
            Mock.Of<ILogger<UpdateMealCommandHandler>>()
        );

        // Act
        var result = await handler.Handle(
            new UpdateMealCommand(aGuid, aVersion, dto),
            new CancellationTokenSource().Token); ;

        // Assert
        mockRepository.Verify(m => m.Update(It.Is<Meal>(m => m == dummyMeal)), Times.Once);
        mockUnitOfWork.Verify(m => m.SaveTransactionAsync(), Times.Once);
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
            Mock.Of<IUnitOfWork>(),
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
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        mockUnitOfWork.Setup(m => m.SaveTransactionAsync()).Throws(new DbUpdateConcurrencyException());
        var handler = new UpdateMealCommandHandler(
            mockRepository.Object,
            mockUnitOfWork.Object,
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

    [Fact]
    public async void Handle_WhenSavingThrowsDomainValidationException_ReturnsNotificationWithNotValidError()
    {
        // Arrange
        var aGuid = Guid.NewGuid();
        const string aVersion = "AAAAAAAA";
        MealDTO dto = new();

        Meal dummyMeal = new(aGuid, DateOnly.FromDateTime(DateTime.Now));

        var mockRepository = new Mock<IMealRepository>();
        mockRepository.Setup(m => m.GetSingleById(It.IsAny<Guid>())).Returns(dummyMeal);
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        mockUnitOfWork.Setup(m => m.SaveTransactionAsync()).Throws(new DomainValidationException(String.Empty));
        var handler = new UpdateMealCommandHandler(
            mockRepository.Object,
            mockUnitOfWork.Object,
            Mock.Of<IOutputMapper<MealDTO, Meal>>(),
            Mock.Of<ILogger<UpdateMealCommandHandler>>()
        );

        // Act
        var result = await handler.Handle(
            new UpdateMealCommand(aGuid, aVersion, dto),
            new CancellationTokenSource().Token);

        // Assert
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorCode.Should().Be(CommandErrorCodes.NotValid);
    }
}
