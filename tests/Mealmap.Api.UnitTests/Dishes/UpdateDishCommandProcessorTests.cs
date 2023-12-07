using Mealmap.Api.Dishes;
using Mealmap.Api.Common;
using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.Common.Validation;
using Mealmap.Domain.DishAggregate;
using Microsoft.Extensions.Logging;

namespace Mealmap.Api.UnitTests.Dishes;

public class UpdateDishCommandProcessorTests
{

    [Fact]
    public async void Process_SavesUpdateAndReturnsDTO()
    {
        // Arrange
        var aGuid = Guid.NewGuid();
        const string aVersion = "AAAAAAAA";
        DishDTO dto = new("fakeDishName");

        Dish dummyDish = new(aGuid, "fakeDishName", null, 2);

        var mockRepository = new Mock<IRepository<Dish>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        mockRepository.Setup(m => m.GetSingleById(It.Is<Guid>(g => g == aGuid))).Returns(dummyDish);
        var processor = new UpdateDishCommandProcessor(
            mockRepository.Object,
            mockUnitOfWork.Object,
            Mock.Of<IOutputMapper<DishDTO, Dish>>(m => m.FromEntity(dummyDish) == dto),
            Mock.Of<ILogger<UpdateDishCommandProcessor>>(),
            new DishDataTransferObjectValidator()
        );

        // Act
        var result = await processor.Process(new UpdateDishCommand(aGuid, aVersion, dto));

        // Assert
        mockRepository.Verify(m => m.Update(It.IsAny<Dish>()), Times.Once);
        mockUnitOfWork.Verify(m => m.SaveTransactionAsync(), Times.Once);
        result.Result.Should().NotBeNull();
    }

    [Fact]
    public async void Process_WhenDishDoesNotExist_ReturnsNotificationWithNotFoundError()
    {
        // Arrange
        var aGuid = Guid.NewGuid();
        const string aVersion = "AAAAAAAA";
        DishDTO dto = new("fakeDishName");

        var mockRepository = new Mock<IRepository<Dish>>();
        mockRepository.Setup(m => m.GetSingleById(It.IsAny<Guid>())).Returns(value: null);
        var processor = new UpdateDishCommandProcessor(
            mockRepository.Object,
            Mock.Of<IUnitOfWork>(),
            Mock.Of<IOutputMapper<DishDTO, Dish>>(),
            Mock.Of<ILogger<UpdateDishCommandProcessor>>(),
            new DishDataTransferObjectValidator()
        );

        // Act
        var result = await processor.Process(new UpdateDishCommand(aGuid, aVersion, dto));

        // Assert
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorCode.Should().Be(CommandErrorCodes.NotFound);
    }

    [Fact]
    public async void Process_WhenSavingThrowsConcurrentUpdateException_ReturnsNotificationWithEtagMismatchError()
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
        var processor = new UpdateDishCommandProcessor(
            mockRepository.Object,
            mockUnitOfWork.Object,
            Mock.Of<IOutputMapper<DishDTO, Dish>>(),
            Mock.Of<ILogger<UpdateDishCommandProcessor>>(),
            new DishDataTransferObjectValidator()
        );

        // Act
        var result = await processor.Process(new UpdateDishCommand(aGuid, aVersion, dto));

        // Assert
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorCode.Should().Be(CommandErrorCodes.VersionMismatch);
    }

    [Fact]
    public async void Process_WhenSavingThrowsDomainValidationException_ReturnsNotificationWithNotValidError()
    {
        // Arrange
        var aGuid = Guid.NewGuid();
        const string aVersion = "AAAAAAAA";
        DishDTO dto = new("fakeDishName");

        Dish dummyDish = new(aGuid, "fakeDishName", null, 2);

        var mockRepository = new Mock<IRepository<Dish>>();
        mockRepository.Setup(m => m.GetSingleById(It.IsAny<Guid>())).Returns(dummyDish);
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        mockUnitOfWork.Setup(m => m.SaveTransactionAsync()).Throws(new DomainValidationException(string.Empty));
        var processor = new UpdateDishCommandProcessor(
            mockRepository.Object,
            mockUnitOfWork.Object,
            Mock.Of<IOutputMapper<DishDTO, Dish>>(),
            Mock.Of<ILogger<UpdateDishCommandProcessor>>(),
            new DishDataTransferObjectValidator()
        );

        // Act
        var result = await processor.Process(new UpdateDishCommand(aGuid, aVersion, dto));

        // Assert
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorCode.Should().Be(CommandErrorCodes.NotValid);
    }
}
