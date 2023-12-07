using Mealmap.Api.Meals;
using Mealmap.Api.Common;
using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.Common.Validation;
using Mealmap.Domain.DishAggregate;
using Mealmap.Domain.MealAggregate;
using Microsoft.Extensions.Logging;

namespace Mealmap.Api.UnitTests.Meals;

public class CreateMealCommandProcessorTests
{
    [Fact]
    public async void Process_WhenMealIsValid_StoresMealAndReturnsDto()
    {
        var repository = new Mock<IMealRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        CreateMealCommandProcessor processor = new(
            repository.Object,
            unitOfWork.Object,
            Mock.Of<IOutputMapper<MealDTO, Meal>>(m => m.FromEntity(It.IsAny<Meal>()) == new MealDTO()),
            Mock.Of<ILogger<CreateMealCommandProcessor>>(),
            new MealDataTransferObjectValidator(Mock.Of<IRepository<Dish>>(repo
                => repo.GetSingleById(It.IsAny<Guid>()) == new Dish("fake")))
        );

        MealDTO dto = new() { DiningDate = DateOnly.FromDateTime(DateTime.Now) };

        var result = await processor.Process(new CreateMealCommand(dto));

        repository.Verify(m => m.Add(It.IsAny<Meal>()), Times.Once);
        unitOfWork.Verify(m => m.SaveTransactionAsync(), Times.Once);
        result.Succeeded.Should().BeTrue();
        result.Result.Should().NotBeNull();
    }

    [Fact]
    public async void Process_WhenValidatorReturnsError_ReturnsNotificationWithNotValidError()
    {
        CreateMealCommandProcessor processor = new(
            Mock.Of<IMealRepository>(),
            Mock.Of<IUnitOfWork>(),
            Mock.Of<IOutputMapper<MealDTO, Meal>>(m => m.FromEntity(It.IsAny<Meal>()) == new MealDTO()),
            Mock.Of<ILogger<CreateMealCommandProcessor>>(),
            new MealDataTransferObjectValidator(Mock.Of<IRepository<Dish>>(repo
                => repo.GetSingleById(It.IsAny<Guid>()) == null))
        );

        MealDTO dto = new()
        {
            DiningDate = DateOnly.FromDateTime(DateTime.Now),
            Courses = [new CourseDTO() { Index = 1, DishId = Guid.NewGuid(), MainCourse = true }]
        };

        var result = await processor.Process(new CreateMealCommand(dto));

        result.Succeeded.Should().BeFalse();
        result.Errors[0].ErrorCode.Should().Be(CommandErrorCodes.NotValid);
    }

    [Fact]
    public async void Process_WhenSavingThrowsDomainValidationException_ReturnsNotificationWithNotValidError()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(m => m.SaveTransactionAsync()).Throws(new DomainValidationException(string.Empty));

        CreateMealCommandProcessor processor = new(
            Mock.Of<IMealRepository>(),
            unitOfWork.Object,
            Mock.Of<IOutputMapper<MealDTO, Meal>>(m => m.FromEntity(It.IsAny<Meal>()) == new MealDTO()),
            Mock.Of<ILogger<CreateMealCommandProcessor>>(),
            new MealDataTransferObjectValidator(Mock.Of<IRepository<Dish>>(repo
                => repo.GetSingleById(It.IsAny<Guid>()) == new Dish("fake")))
        );

        MealDTO dto = new() { DiningDate = DateOnly.FromDateTime(DateTime.Now) };

        var result = await processor.Process(new CreateMealCommand(dto));

        result.Succeeded.Should().BeFalse();
        result.Errors[0].ErrorCode.Should().Be(CommandErrorCodes.NotValid);
    }
}
