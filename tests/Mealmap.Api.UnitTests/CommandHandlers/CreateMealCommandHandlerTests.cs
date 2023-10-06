using Mealmap.Api.CommandHandlers;
using Mealmap.Api.Commands;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Api.DataTransferObjectValidators;
using Mealmap.Api.OutputMappers;
using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.Common.Validation;
using Mealmap.Domain.DishAggregate;
using Mealmap.Domain.MealAggregate;
using Microsoft.Extensions.Logging;

namespace Mealmap.Api.UnitTests.CommandHandlers;

public class CreateMealCommandHandlerTests
{
    [Fact]
    public async void Handle_WhenMealIsValid_StoresMealAndReturnsDto()
    {
        var repository = new Mock<IMealRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        CreateMealCommandHandler handler = new(
            repository.Object,
            unitOfWork.Object,
            Mock.Of<IOutputMapper<MealDTO, Meal>>(m => m.FromEntity(It.IsAny<Meal>()) == new MealDTO()),
            Mock.Of<ILogger<CreateMealCommandHandler>>(),
            new MealDataTransferObjectValidator(Mock.Of<IRepository<Dish>>(repo
                => repo.GetSingleById(It.IsAny<Guid>()) == new Dish("fake")))
        );

        MealDTO dto = new() { DiningDate = DateOnly.FromDateTime(DateTime.Now) };

        var result = await handler.Handle(
            new CreateMealCommand(dto),
            new CancellationTokenSource().Token
        );

        repository.Verify(m => m.Add(It.IsAny<Meal>()), Times.Once);
        unitOfWork.Verify(m => m.SaveTransactionAsync(), Times.Once);
        result.Succeeded.Should().BeTrue();
        result.Result.Should().NotBeNull();
    }

    [Fact]
    public async void Handle_WhenValidatorReturnsError_ReturnsNotificationWithNotValidError()
    {
        CreateMealCommandHandler handler = new(
            Mock.Of<IMealRepository>(),
            Mock.Of<IUnitOfWork>(),
            Mock.Of<IOutputMapper<MealDTO, Meal>>(m => m.FromEntity(It.IsAny<Meal>()) == new MealDTO()),
            Mock.Of<ILogger<CreateMealCommandHandler>>(),
            new MealDataTransferObjectValidator(Mock.Of<IRepository<Dish>>(repo
                => repo.GetSingleById(It.IsAny<Guid>()) == null))
        );

        MealDTO dto = new()
        {
            DiningDate = DateOnly.FromDateTime(DateTime.Now),
            Courses = new[] { new CourseDTO() { Index = 1, DishId = Guid.NewGuid(), MainCourse = true } }
        };

        var result = await handler.Handle(
            new CreateMealCommand(dto),
            new CancellationTokenSource().Token
        );

        result.Succeeded.Should().BeFalse();
        result.Errors[0].ErrorCode.Should().Be(CommandErrorCodes.NotValid);
    }

    [Fact]
    public async void Handle_WhenSavingThrowsDomainValidationException_ReturnsNotificationWithNotValidError()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(m => m.SaveTransactionAsync()).Throws(new DomainValidationException(String.Empty));

        CreateMealCommandHandler handler = new(
            Mock.Of<IMealRepository>(),
            unitOfWork.Object,
            Mock.Of<IOutputMapper<MealDTO, Meal>>(m => m.FromEntity(It.IsAny<Meal>()) == new MealDTO()),
            Mock.Of<ILogger<CreateMealCommandHandler>>(),
            new MealDataTransferObjectValidator(Mock.Of<IRepository<Dish>>(repo
                => repo.GetSingleById(It.IsAny<Guid>()) == new Dish("fake")))
        );

        MealDTO dto = new() { DiningDate = DateOnly.FromDateTime(DateTime.Now) };

        var result = await handler.Handle(
            new CreateMealCommand(dto),
            new CancellationTokenSource().Token
        );

        result.Succeeded.Should().BeFalse();
        result.Errors[0].ErrorCode.Should().Be(CommandErrorCodes.NotValid);
    }
}
