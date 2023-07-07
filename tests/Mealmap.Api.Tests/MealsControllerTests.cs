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

            _controller = new MealsController(
                _logger,
                _mealRepository,
                _dishRepository,
                new MealMapper(
                    new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>()).CreateMapper()
                    , _dishRepository));

            fakeData(_mealRepository, _dishRepository);
        }

        private void fakeData(FakeMealRepository mealRepository, FakeDishRepository dishRepository)
        {
            Dish krabbyPatty = new("Krabby Patty") { Id = new Guid("00000000-0000-0000-0000-000000000001") };
            dishRepository.Create(krabbyPatty);

            Meal yesterdaysMeal = new Meal()
            {
                Id = new Guid("11111111-1111-1111-1111-111111111111"),
                Date = new DateOnly(2020, 1, 1),
                Dish = krabbyPatty
            };
            mealRepository.Create(yesterdaysMeal);
        }

        [Fact]
        public void GetMeals_ReturnsMealDTOs()
        {
            var result = _controller.GetMeals();

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

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void PostMeal_WhenMealIsValid_ReturnsMealWithId()
        {
            var dishId = _dishRepository.ElementAt(0).Key;
            MealDTO mealDto = new() { Date = new DateOnly(2020, 1, 2), Dish = dishId };

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
            MealDTO mealDto = new() { Date = new DateOnly(2020,1,2), Dish = dishId };

            _ = _controller.PostMeal(mealDto);

            _mealRepository.Should().NotBeEmpty().And.HaveCountGreaterThan(1);
        }

        [Fact]
        public void PostMeal_WhenMealHasId_ReplacesId()
        {
            Guid someGuid = Guid.NewGuid();
            MealDTO mealDto = new() { Id = someGuid };

            var result = _controller.PostMeal(mealDto);

            var value = (MealDTO)((CreatedAtActionResult)result.Result!).Value!;
            value.Id.Should().NotBeNull().And.NotBeEmpty().And.NotBe(someGuid);
        }
    }
}
