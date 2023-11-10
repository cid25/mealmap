using Mealmap.Api.Dishes;
using Mealmap.Api.Shared;
using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.Common.Validation;
using Mealmap.Domain.DishAggregate;
using Microsoft.Extensions.Logging;

namespace Mealmap.Api.UnitTests.Dishes;

public class CreateDishCommandProcessorTests
{
    [Fact]
    public async void Process_WhenDishIsValid_StoresMealAndReturnsDto()
    {
        var repository = new Mock<IRepository<Dish>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        CreateDishCommandProcessor processor = new(
            repository.Object,
            unitOfWork.Object,
            Mock.Of<IOutputMapper<DishDTO, Dish>>(m => m.FromEntity(It.IsAny<Dish>()) == new DishDTO("fakeName")),
            Mock.Of<ILogger<CreateDishCommandProcessor>>(),
            new DishDataTransferObjectValidator()
        );

        DishDTO dto = new("fakeName");

        var result = await processor.Process(new CreateDishCommand(dto));

        repository.Verify(m => m.Add(It.IsAny<Dish>()), Times.Once);
        unitOfWork.Verify(m => m.SaveTransactionAsync(), Times.Once);
        result.Succeeded.Should().BeTrue();
        result.Result.Should().NotBeNull();
    }

    [Fact]
    public async void Process_WhenSavingThrowsDomainValidationException_ReturnsNotificationWithNotValidError()
    {
        var repository = new Mock<IRepository<Dish>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(m => m.SaveTransactionAsync()).Throws(new DomainValidationException(string.Empty));
        CreateDishCommandProcessor processor = new(
            repository.Object,
            unitOfWork.Object,
            Mock.Of<IOutputMapper<DishDTO, Dish>>(m => m.FromEntity(It.IsAny<Dish>()) == new DishDTO("fakeName")),
            Mock.Of<ILogger<CreateDishCommandProcessor>>(),
            new DishDataTransferObjectValidator()
        );

        DishDTO dto = new("fakeName");

        var result = await processor.Process(new CreateDishCommand(dto));

        result.Succeeded.Should().BeFalse();
        result.Errors[0].ErrorCode.Should().Be(CommandErrorCodes.NotValid);
    }
}
