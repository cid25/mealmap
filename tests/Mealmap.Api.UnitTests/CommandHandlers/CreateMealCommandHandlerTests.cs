using FluentAssertions;
using Mealmap.Api.CommandHandlers;
using Mealmap.Api.Commands;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Api.OutputMappers;
using Mealmap.Domain.MealAggregate;
using Microsoft.Extensions.Logging;
using Moq;

namespace Mealmap.Api.UnitTests.CommandHandlers;

public class CreateMealCommandHandlerTests
{
    [Fact]
    public async void PostMeal_WhenMealIsValid_StoresMealAndReturnsDto()
    {
        var repository = new Mock<IMealRepository>();
        CreateMealCommandHandler handler = new(
            repository.Object,
            Mock.Of<IOutputMapper<MealDTO, Meal>>(m => m.FromEntity(It.IsAny<Meal>()) == new MealDTO()),
            Mock.Of<ILogger<CreateMealCommandHandler>>()
        );

        MealDTO mealDto = new() { DiningDate = new DateOnly(2020, 1, 2) };

        var result = await handler.Handle(
            new CreateMealCommand(mealDto),
            new CancellationTokenSource().Token
        );

        repository.Verify(m => m.Add(It.IsAny<Meal>()), Times.Once);
        result.Success.Should().BeTrue();
        result.Result.Should().NotBeNull();
    }
}
