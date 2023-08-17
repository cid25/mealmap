using AutoMapper;
using Mealmap.Api.Commands;
using Mealmap.Api.Controllers;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Api.OutputMappers;
using Mealmap.Domain.DishAggregate;
using Mealmap.Domain.MealAggregate;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Mealmap.Api.UnitTests.Controllers;

public class MealsControllerTests
{
    private readonly ILogger<MealsController> _logger = new Mock<ILogger<MealsController>>().Object;
    private readonly FakeMealRepository _mealRepository = new();
    private readonly FakeDishRepository _dishRepository = new();
    private readonly MealsController _controller;
    private readonly MealOutputMapper _outputMapper;

    public MealsControllerTests()
    {
        _outputMapper = new MealOutputMapper(
          new MapperConfiguration(cfg => cfg.AddProfile<AutomapperProfile>()).CreateMapper());

        _controller = new MealsController(
            _logger,
            _mealRepository,
            _outputMapper,
            Mock.Of<IRequestContext>(),
            Mock.Of<IMediator>()
        );

        fakeData();
    }

    private void fakeData()
    {
        Dish krabbyPatty = new("Krabby Patty") { Description = null, Servings = 2 };
        _dishRepository.Add(krabbyPatty);

        Meal meal = new(diningDate: DateOnly.FromDateTime(DateTime.Now));
        meal.AddCourse(index: 1, mainCourse: true, dishId: krabbyPatty.Id);
        _mealRepository.Add(meal);
    }

    [Fact]
    public void GetMeals_ReturnsMealDTOs()
    {
        var result = _controller.GetMeals(null, null);

        result.Should().BeOfType<ActionResult<IEnumerable<MealDTO>>>();
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public void GetMeal_WhenMealWithIdExists_ReturnsMeal()
    {
        var guid = _mealRepository.ElementAt(0).Key;
        var result = _controller.GetMeal(guid);

        result.Should().BeOfType<ActionResult<MealDTO>>();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void GetMeal_WhenMealWithIdNotExists_ReturnsNotFound()
    {
        const string nonExistingGuid = "99999999-9999-9999-9999-999999999999";
        var result = _controller.GetMeal(new Guid(nonExistingGuid));

        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async void PostMeal_WhenMealIsValid_ReturnsMeal()
    {
        MealDTO mealDto = new() { DiningDate = new DateOnly(2020, 1, 2) };
        CommandNotification<MealDTO> notification = new()
        {
            Result = mealDto
        };
        var mediatorMock = new Mock<IMediator>();
        mediatorMock.Setup(m => m.Send(It.IsAny<CreateMealCommand>(), It.IsAny<CancellationToken>()).Result).Returns(notification);
        var controller = new MealsController(
            _logger,
            _mealRepository,
            _outputMapper,
            Mock.Of<IRequestContext>(),
            mediatorMock.Object
        );

        var result = await controller.PostMeal(mealDto);

        result.Result.Should().BeOfType<CreatedAtActionResult>();
        ((CreatedAtActionResult)result.Result!).Value.Should().BeOfType<MealDTO>();
        var value = (MealDTO)((CreatedAtActionResult)result.Result!).Value!;
    }

    [Fact]
    public async void PostMeal_WhenMealIsInvalid_ReturnsBadRequest()
    {
        MealDTO mealDto = new();
        CommandNotification<MealDTO> notification = new();
        notification.Errors.Add(new CommandError(CommandErrorCodes.NotValid));
        var mediatorMock = new Mock<IMediator>();
        mediatorMock.Setup(m => m.Send(It.IsAny<CreateMealCommand>(), It.IsAny<CancellationToken>()).Result)
            .Returns(notification);
        var controller = new MealsController(
            _logger,
            _mealRepository,
            _outputMapper,
            Mock.Of<IRequestContext>(),
            mediatorMock.Object
        );

        var result = await controller.PostMeal(mealDto);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }


    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async void PutMeal_WhenIfMatchHeaderNotSet_ReturnsPreconditionRequired(string? header)
    {
        var controller = new MealsController(
            _logger,
            Mock.Of<IMealRepository>(),
            Mock.Of<IOutputMapper<MealDTO, Meal>>(),
            Mock.Of<IRequestContext>(m => m.IfMatchHeader == header),
            Mock.Of<IMediator>()
        );

        MealDTO dto = new();
        var result = await controller.PutMeal(Guid.NewGuid(), dto);

        result.Result.Should().BeOfType<StatusCodeResult>();
        ((StatusCodeResult)result.Result!).StatusCode.Should().Be(428);
    }

    [Fact]
    public async void PutMeal_WhenVersionDoesNotMatch_ReturnsPreconditionFailed()
    {
        CommandNotification<MealDTO> notification = new();
        notification.Errors.Add(new CommandError(CommandErrorCodes.VersionMismatch));
        var mediatorMock = new Mock<IMediator>();
        mediatorMock.Setup(m => m.Send(It.IsAny<UpdateMealCommand>(), It.IsAny<CancellationToken>()).Result)
            .Returns(notification);
        var controller = new MealsController(
            _logger,
            Mock.Of<IMealRepository>(),
            Mock.Of<IOutputMapper<MealDTO, Meal>>(),
            Mock.Of<IRequestContext>(m => m.IfMatchHeader == "fakeVersion"),
            mediatorMock.Object
        );

        MealDTO dto = new();
        var result = await controller.PutMeal(Guid.NewGuid(), dto);

        result.Result.Should().BeOfType<StatusCodeResult>();
        ((StatusCodeResult)result.Result!).StatusCode.Should().Be(412);
    }

    [Fact]
    public async void PutMeal_WhenDishNotFound_ReturnsNotFound()
    {
        CommandNotification<MealDTO> notification = new();
        notification.Errors.Add(new CommandError(CommandErrorCodes.NotFound));
        var mediatorMock = new Mock<IMediator>();
        mediatorMock.Setup(m => m.Send(It.IsAny<UpdateMealCommand>(), It.IsAny<CancellationToken>()).Result)
            .Returns(notification);
        var controller = new MealsController(
            _logger,
            Mock.Of<IMealRepository>(),
            Mock.Of<IOutputMapper<MealDTO, Meal>>(),
            Mock.Of<IRequestContext>(m => m.IfMatchHeader == "fakeVersion"),
            mediatorMock.Object
        );

        MealDTO dto = new();
        var result = await controller.PutMeal(Guid.NewGuid(), dto);

        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async void PutMeal_WhenValidationError_ReturnsBadRequest()
    {
        CommandNotification<MealDTO> notification = new();
        notification.Errors.Add(new CommandError(CommandErrorCodes.NotValid));
        var mediatorMock = new Mock<IMediator>();
        mediatorMock.Setup(m => m.Send(It.IsAny<UpdateMealCommand>(), It.IsAny<CancellationToken>()).Result)
            .Returns(notification);
        var controller = new MealsController(
            _logger,
            Mock.Of<IMealRepository>(),
            Mock.Of<IOutputMapper<MealDTO, Meal>>(),
            Mock.Of<IRequestContext>(m => m.IfMatchHeader == "fakeVersion"),
            mediatorMock.Object
        );

        MealDTO dto = new();
        var result = await controller.PutMeal(Guid.NewGuid(), dto);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void DeleteMeal_WhenMealExists_ReturnsOkAndDish()
    {
        var dish = _mealRepository.GetAll().First();

        var result = _controller.DeleteMeal(dish.Id);

        result.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)result.Result!).Value.Should().BeOfType<MealDTO>();
    }

    [Fact]
    public void DeleteMeal_WhenMealDoesntExist_ReturnsNotFound()
    {
        var nonExistingMealGuid = new Guid("99999999-9999-9999-9999-999999999999");

        var result = _controller.DeleteMeal(nonExistingMealGuid);

        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }
}
