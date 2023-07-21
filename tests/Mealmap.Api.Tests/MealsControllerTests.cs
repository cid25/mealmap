using AutoMapper;
using FluentAssertions;
using Mealmap.Api.Controllers;
using Mealmap.Api.DataTransfer;
using Mealmap.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Mealmap.Api.UnitTests
{
    public class MealsControllerTests
    {
        private readonly ILogger<MealsController> _logger;
        private readonly FakeMealRepository _mealRepository;
        private readonly FakeDishRepository _dishRepository;
        private readonly MealsController _controller;

        public MealsControllerTests()
        {
            _logger = (new Mock<ILogger<MealsController>>().Object);
            _dishRepository = new FakeDishRepository();
            _mealRepository = new FakeMealRepository();

            var mapper = new MealMapper(
                Mock.Of<ILogger<MealMapper>>(),
                new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>()).CreateMapper(),
                _dishRepository);

            _controller = new MealsController(
                _logger,
                _mealRepository,
                _dishRepository,
                mapper);

            fakeData();
        }

        private void fakeData()
        {
            Dish krabbyPatty = new("Krabby Patty") { Id = new Guid("00000000-0000-0000-0000-000000000001") };
            _dishRepository.Add(krabbyPatty);

            Meal yesterdaysMeal = new Meal()
            {
                Id = new Guid("11111111-1111-1111-1111-111111111111"),
                DiningDate = new DateOnly(2020, 1, 1),
                Dish = krabbyPatty
            };
            _mealRepository.Add(yesterdaysMeal);
        }

        [Fact]
        public void GetMeals_ReturnsMealDTOs()
        {
            var result = _controller.GetMeals(null, null);

            result.Should().BeOfType<ActionResult<IEnumerable<MealDTO>>>();
            result.Value.Should().HaveCountGreaterThan(0);
            result.Value.Should().NotContain(x => x.Id == null || x.Id == Guid.Empty);
        }

        [Fact]
        public void GetMeal_WhenMealWithIdExists_ReturnsMeal()
        {
            Guid guid = _mealRepository.ElementAt(0).Key;
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
            var dishId = _dishRepository.ElementAt(0).Key;
            MealDTO mealDto = new() { DiningDate = new DateOnly(2020, 1, 2), DishId = dishId };

            var result = _controller.PostMeal(mealDto);

            result.Result.Should().BeOfType<CreatedAtActionResult>();
            ((CreatedAtActionResult)result.Result!).Value.Should().BeOfType<MealDTO>();
            var value = (MealDTO)((CreatedAtActionResult)result.Result!).Value!;
            value.Id.Should().NotBeNull().And.NotBeEmpty();
        }

        [Fact]
        public void PostMeal_WhenMealIsValid_StoresMeal()
        {
            var dishId = _dishRepository.ElementAt(0).Key;
            MealDTO mealDto = new() { DiningDate = new DateOnly(2020, 1, 2), DishId = dishId };

            _ = _controller.PostMeal(mealDto);

            _mealRepository.Should().NotBeEmpty().And.HaveCountGreaterThan(1);
        }

        [Fact]
        public void PostMeal_WhenMealHasId_ReturnsBadRequest()
        {
            Guid someGuid = Guid.NewGuid();
            MealDTO mealDto = new() { Id = someGuid };

            var result = _controller.PostMeal(mealDto);

            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void PostMeal_WhenDishDoesntExist_ReturnsBadRequest()
        {
            Guid nonExistingDishGuid = Guid.NewGuid();
            MealDTO mealDto = new()
            {
                DiningDate = DateOnly.FromDateTime(DateTime.Now),
                DishId = nonExistingDishGuid
            };

            var result = _controller.PostMeal(mealDto);

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
            Guid nonExistingMealGuid = new Guid("99999999-9999-9999-9999-999999999999");

            var result = _controller.DeleteMeal(nonExistingMealGuid);

            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}
