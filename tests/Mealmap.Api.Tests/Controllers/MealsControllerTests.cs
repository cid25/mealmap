using AutoMapper;
using FluentAssertions;
using Mealmap.Api.Controllers;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Api.OutputMappers;
using Mealmap.Domain.Common;
using Mealmap.Domain.DishAggregate;
using Mealmap.Domain.MealAggregate;
using Mealmap.Infrastructure.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Mealmap.Api.UnitTests.Controllers;

public class MealsControllerTests
{
    private readonly ILogger<MealsController> _logger;
    private readonly FakeMealRepository _mealRepository;
    private readonly FakeDishRepository _dishRepository;
    private readonly MealsController _controller;
    private readonly MealOutputMapper _outputMapper;

    public MealsControllerTests()
    {
        _logger = new Mock<ILogger<MealsController>>().Object;
        _dishRepository = new FakeDishRepository();
        _mealRepository = new FakeMealRepository();

        _outputMapper = new MealOutputMapper(
            new MapperConfiguration(cfg => cfg.AddProfile<AutomapperProfile>()).CreateMapper());

        _controller = new MealsController(
            _logger,
            _mealRepository,
            new MealService(_dishRepository),
            _outputMapper);

        fakeData();
    }

    private void fakeData()
    {
        Dish krabbyPatty = new("Krabby Patty");
        _dishRepository.Add(krabbyPatty);

        var meal = new Meal(DateOnly.FromDateTime(DateTime.Now));
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
    public void PostMeal_WhenMealIsValid_ReturnsMealWithId()
    {
        MealDTO mealDto = new() { DiningDate = new DateOnly(2020, 1, 2) };

        var result = _controller.PostMeal(mealDto);

        result.Result.Should().BeOfType<CreatedAtActionResult>();
        ((CreatedAtActionResult)result.Result!).Value.Should().BeOfType<MealDTO>();
        var value = (MealDTO)((CreatedAtActionResult)result.Result!).Value!;
        value.Id.Should().NotBeNull().And.NotBeEmpty();
    }

    [Fact]
    public void PostMeal_WhenMealIsValid_StoresMeal()
    {
        MealDTO mealDto = new() { DiningDate = new DateOnly(2020, 1, 2) };

        _controller.PostMeal(mealDto);

        _mealRepository.Should().NotBeEmpty().And.HaveCountGreaterThan(1);
    }

    [Fact]
    public void PostMeal_WhenServiceThrowsDomainValidationException_ReturnsBadRequest()
    {
        Meal dummy = new Meal(DateOnly.FromDateTime(DateTime.Now));
        var serviceMock = new Mock<IMealService>();
        serviceMock.Setup(m => m.CreateMeal(It.IsAny<DateOnly>()))
            .Returns(dummy);
        serviceMock.Setup(m => m.AddCourseToMeal(It.IsAny<Meal>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<Guid>()))
            .Throws(new DomainValidationException(""));
        var controller = new MealsController(
            _logger,
            _mealRepository,
            serviceMock.Object,
            _outputMapper
        );
        MealDTO mealDto = new()
        {
            Courses = new CourseDTO[1] { new CourseDTO() { Index = 1, DishId = Guid.NewGuid(), MainCourse = true } }
        };

        var result = controller.PostMeal(mealDto);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void PostMeal_WhenRepositoryThrowsDbUpdateException_ReturnsBadRequest()
    {
        var serviceMock = new Mock<IMealService>();
        serviceMock.Setup(m => m.CreateMeal(It.IsAny<DateOnly>())).Returns(new Meal(DateOnly.FromDateTime(DateTime.Now)));
        var repositoryMock = new Mock<IMealRepository>();
        repositoryMock.Setup(m => m.Add(It.IsAny<Meal>())).Throws(new ConcurrentUpdateException(""));
        var controller = new MealsController(
            _logger,
            repositoryMock.Object,
            serviceMock.Object,
            _outputMapper
        );

        MealDTO mealDto = new();

        var result = controller.PostMeal(mealDto);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void Delete_WhenMealExists_ReturnsOkAndDish()
    {
        var dish = _mealRepository.GetAll().First();

        var result = _controller.DeleteMeal(dish.Id);

        result.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)result.Result!).Value.Should().BeOfType<MealDTO>();
    }

    [Fact]
    public void Delete_WhenMealDoesntExist_ReturnsNotFound()
    {
        var nonExistingMealGuid = new Guid("99999999-9999-9999-9999-999999999999");

        var result = _controller.DeleteMeal(nonExistingMealGuid);

        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }
}
