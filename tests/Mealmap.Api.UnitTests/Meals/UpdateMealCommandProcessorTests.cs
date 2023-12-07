using Mealmap.Api.Meals;
using Mealmap.Api.Common;
using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.Common.Validation;
using Mealmap.Domain.DishAggregate;
using Mealmap.Domain.MealAggregate;
using Microsoft.Extensions.Logging;

namespace Mealmap.Api.UnitTests.Meals;

public class UpdateMealCommandProcessorTests
{

    [Fact]
    public async void Process_SavesUpdateAndReturnsDTO()
    {
        // Arrange
        var aGuid = Guid.NewGuid();
        const string aVersion = "AAAAAAAA";
        MealDTO dto = new();

        Meal dummyMeal = new(aGuid, DateOnly.FromDateTime(DateTime.Now));

        var mockRepository = new Mock<IMealRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        mockRepository.Setup(m => m.GetSingleById(It.Is<Guid>(g => g == aGuid))).Returns(dummyMeal);
        var processor = new UpdateMealCommandProcessor(
            mockRepository.Object,
            mockUnitOfWork.Object,
            Mock.Of<IOutputMapper<MealDTO, Meal>>(m => m.FromEntity(dummyMeal) == dto),
            Mock.Of<ILogger<UpdateMealCommandProcessor>>(),
            new MealDataTransferObjectValidator(Mock.Of<IRepository<Dish>>(repo
                => repo.GetSingleById(It.IsAny<Guid>()) == new Dish("fake")))
        );

        // Act
        var result = await processor.Process(new UpdateMealCommand(aGuid, aVersion, dto));

        // Assert
        mockRepository.Verify(m => m.Update(It.Is<Meal>(m => m == dummyMeal)), Times.Once);
        mockUnitOfWork.Verify(m => m.SaveTransactionAsync(), Times.Once);
        result.Result.Should().NotBeNull();
    }

    [Fact]
    public async void Process_WhenMealDoesNotExist_ReturnsNotificationWithNotFoundError()
    {
        // Arrange
        var aGuid = Guid.NewGuid();
        const string aVersion = "AAAAAAAA";
        MealDTO dto = new();

        var mockRepository = new Mock<IMealRepository>();
        mockRepository.Setup(m => m.GetSingleById(It.Is<Guid>(g => g == aGuid))).Returns(value: null);
        var processor = new UpdateMealCommandProcessor(
            mockRepository.Object,
            Mock.Of<IUnitOfWork>(),
            Mock.Of<IOutputMapper<MealDTO, Meal>>(),
            Mock.Of<ILogger<UpdateMealCommandProcessor>>(),
            new MealDataTransferObjectValidator(Mock.Of<IRepository<Dish>>(repo
                => repo.GetSingleById(It.IsAny<Guid>()) == new Dish("fake")))
        );

        // Act
        var result = await processor.Process(new UpdateMealCommand(aGuid, aVersion, dto));

        // Assert
        result.Errors.Should().ContainSingle();
        result.Errors[0].ErrorCode.Should().Be(CommandErrorCodes.NotFound);
    }

    [Fact]
    public async void Process_WhenValidatorReturnsError_ReturnsNotificationWithNotValidError()
    {
        // Arrange
        var aGuid = Guid.NewGuid();
        const string aVersion = "AAAAAAAA";

        Meal dummyMeal = new(aGuid, DateOnly.FromDateTime(DateTime.Now));

        var processor = new UpdateMealCommandProcessor(
            Mock.Of<IMealRepository>(m => m.GetSingleById(It.IsAny<Guid>()) == dummyMeal),
            Mock.Of<IUnitOfWork>(),
            Mock.Of<IOutputMapper<MealDTO, Meal>>(),
            Mock.Of<ILogger<UpdateMealCommandProcessor>>(),
            new MealDataTransferObjectValidator(Mock.Of<IRepository<Dish>>(repo
                => repo.GetSingleById(It.IsAny<Guid>()) == null))
        );

        MealDTO dto = new()
        {
            DiningDate = DateOnly.FromDateTime(DateTime.Now),
            Courses = [new() { Index = 1, DishId = Guid.NewGuid(), MainCourse = true }]
        };

        // Act
        var result = await processor.Process(new UpdateMealCommand(aGuid, aVersion, dto));

        // Assert
        result.Errors.Should().ContainSingle();
        result.Errors[0].ErrorCode.Should().Be(CommandErrorCodes.NotValid);
    }

    [Fact]
    public async void Process_WhenSavingThrowsConcurrentUpdateException_ReturnsNotificationWithEtagMismatchError()
    {
        // Arrange
        var aGuid = Guid.NewGuid();
        const string aVersion = "AAAAAAAA";
        MealDTO dto = new();

        Meal dummyMeal = new(aGuid, DateOnly.FromDateTime(DateTime.Now));

        var mockUnitOfWork = new Mock<IUnitOfWork>();
        mockUnitOfWork.Setup(m => m.SaveTransactionAsync()).Throws(new ConcurrentUpdateException());
        var processor = new UpdateMealCommandProcessor(
            Mock.Of<IMealRepository>(m => m.GetSingleById(It.IsAny<Guid>()) == dummyMeal),
            mockUnitOfWork.Object,
            Mock.Of<IOutputMapper<MealDTO, Meal>>(),
            Mock.Of<ILogger<UpdateMealCommandProcessor>>(),
            new MealDataTransferObjectValidator(Mock.Of<IRepository<Dish>>(repo
                => repo.GetSingleById(It.IsAny<Guid>()) == new Dish("fake")))
        );

        // Act
        var result = await processor.Process(new UpdateMealCommand(aGuid, aVersion, dto));

        // Assert
        result.Errors.Should().ContainSingle();
        result.Errors[0].ErrorCode.Should().Be(CommandErrorCodes.VersionMismatch);
    }

    [Fact]
    public async void Process_WhenSavingThrowsDomainValidationException_ReturnsNotificationWithNotValidError()
    {
        // Arrange
        var aGuid = Guid.NewGuid();
        const string aVersion = "AAAAAAAA";
        MealDTO dto = new();

        Meal dummyMeal = new(aGuid, DateOnly.FromDateTime(DateTime.Now));

        var mockUnitOfWork = new Mock<IUnitOfWork>();
        mockUnitOfWork.Setup(m => m.SaveTransactionAsync()).Throws(new DomainValidationException(string.Empty));
        var processor = new UpdateMealCommandProcessor(
            Mock.Of<IMealRepository>(m => m.GetSingleById(It.IsAny<Guid>()) == dummyMeal),
            mockUnitOfWork.Object,
            Mock.Of<IOutputMapper<MealDTO, Meal>>(),
            Mock.Of<ILogger<UpdateMealCommandProcessor>>(),
            new MealDataTransferObjectValidator(Mock.Of<IRepository<Dish>>(repo
                => repo.GetSingleById(It.IsAny<Guid>()) == new Dish("fake")))
        );

        // Act
        var result = await processor.Process(new UpdateMealCommand(aGuid, aVersion, dto));

        // Assert
        result.Errors.Should().ContainSingle();
        result.Errors[0].ErrorCode.Should().Be(CommandErrorCodes.NotValid);
    }
}
