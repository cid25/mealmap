using Mealmap.Api.CommandHandlers;
using Mealmap.Api.Commands;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Api.OutputMappers;
using Mealmap.Domain;
using Mealmap.Domain.DishAggregate;
using Mealmap.Domain.Seedwork.Validation;
using Microsoft.Extensions.Logging;

namespace Mealmap.Api.UnitTests.CommandHandlers;

public class CreateDishCommandHandlerTests
{
    [Fact]
    public async void Handle_WhenDishIsValid_StoresMealAndReturnsDto()
    {
        var repository = new Mock<IDishRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        CreateDishCommandHandler handler = new(
            repository.Object,
            unitOfWork.Object,
            Mock.Of<IOutputMapper<DishDTO, Dish>>(m => m.FromEntity(It.IsAny<Dish>()) == new DishDTO("fakeName")),
            Mock.Of<ILogger<CreateDishCommandHandler>>()
        );

        DishDTO dto = new("fakeName");

        var result = await handler.Handle(
            new CreateDishCommand(dto),
            new CancellationTokenSource().Token
        );

        repository.Verify(m => m.Add(It.IsAny<Dish>()), Times.Once);
        unitOfWork.Verify(m => m.SaveTransactionAsync(), Times.Once);
        result.Success.Should().BeTrue();
        result.Result.Should().NotBeNull();
    }

    [Fact]
    public async void Handle_WhenSavingThrowsDomainValidationException_ReturnsNotificationWithNotValidError()
    {
        var repository = new Mock<IDishRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(m => m.SaveTransactionAsync()).Throws(new DomainValidationException(String.Empty));
        CreateDishCommandHandler handler = new(
            repository.Object,
            unitOfWork.Object,
            Mock.Of<IOutputMapper<DishDTO, Dish>>(m => m.FromEntity(It.IsAny<Dish>()) == new DishDTO("fakeName")),
            Mock.Of<ILogger<CreateDishCommandHandler>>()
        );

        DishDTO dto = new("fakeName");

        var result = await handler.Handle(
            new CreateDishCommand(dto),
            new CancellationTokenSource().Token
        );

        result.Success.Should().BeFalse();
        result.Errors[0].ErrorCode.Should().Be(CommandErrorCodes.NotValid);
    }
}
