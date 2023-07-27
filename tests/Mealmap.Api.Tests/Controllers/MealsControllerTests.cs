using AutoMapper;
using FluentAssertions;
using Mealmap.Api.Controllers;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Api.InputMappers;
using Mealmap.Api.OutputMappers;
using Mealmap.Domain.DishAggregate;
using Mealmap.Domain.Exceptions;
using Mealmap.Domain.MealAggregate;
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

        var _outputMapper = new MealOutputMapper(
            new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>()).CreateMapper());

        _controller = new MealsController(
            _logger,
            _mealRepository,
            Mock.Of<IInputMapper<Meal, MealDTO>>(m => m.FromDataTransferObject(It.IsAny<MealDTO>()) == new Meal(DateOnly.FromDateTime(DateTime.Now))),
            _outputMapper);

        fakeData();
    }

    private void fakeData()
    {
        Dish krabbyPatty = new("Krabby Patty") { Id = Guid.NewGuid() };
        _dishRepository.Add(krabbyPatty);

        var meal = new Meal(
            id: new Guid("11111111-1111-1111-1111-111111111111"),
            diningDate: new DateOnly(2020, 1, 1)
        );
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
    public void PostMeal_WhenMapperThrowsDomainValidationException_ReturnsBadRequest()
    {
        var inputMapper = new Mock<IInputMapper<Meal, MealDTO>>();
        inputMapper.Setup(m => m.FromDataTransferObject(It.IsAny<MealDTO>())).Throws(new DomainValidationException(""));
        var controller = new MealsController(
            _logger,
            _mealRepository,
            inputMapper.Object,
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
