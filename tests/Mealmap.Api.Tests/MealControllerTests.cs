using AutoMapper;
using FluentAssertions;
using Mealmap.Api.Controllers;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Model;
using Microsoft.AspNetCore.Mvc;

namespace Mealmap.Api.Tests
{
    public class MealControllerTests
    {
        private readonly FakeMealRepository _repository;
        private readonly MealsController _controller;

        public MealControllerTests()
        {
            _repository = new FakeMealRepository();
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<MealMapperProfile>());
            _controller = new MealsController(_repository, mapperConfig.CreateMapper());

            const string firstGuid = "00000000-0000-0000-0000-000000000001";
            var cheeseburger = new Meal(id: new Guid(firstGuid), name: "Cheeseburger");
            _repository.Create(cheeseburger);
        }

        [Fact]
        public void GetMeal_WhenGivenExistingId_ReturnsListMealDtos()
        {
            var result = _controller.GetMeals();

            result.Should().BeOfType<ActionResult<IEnumerable<MealDto>>>();
        }

        [Fact]
        public void GetMeal_WhenGivenExistingId_ReturnsMealDto()
        {
            Guid guid = _repository.ElementAt(0).Key;
            var result = _controller.GetMeal(guid);

            result.Should().BeOfType<ActionResult<MealDto>>();
            result.Value.Should().NotBeNull();
            result.Value!.Id.Should().NotBeEmpty();
        }

        [Fact]
        public void GetMeal_WhenGivenNonExistingId_ReturnsNotFound()
        {
            const string nonExistingGuid = "99999999-9999-9999-9999-999999999999";
            var result = _controller.GetMeal(new Guid(nonExistingGuid));

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void PostMeal_WhenGivenValidMeal_ReturnsOk()
        {
            const string someMealName = "Cheeseburger";
            MealDto mealDto = new(Guid.NewGuid(), someMealName);

            var result = _controller.PostMeal(mealDto);

            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public void PostMeal_WhenGivenValidMeal_StoresMeal()
        {;
            const string someMealName = "Cheeseburger";
            MealDto mealDto = new(Guid.NewGuid(), someMealName);

            _ = _controller.PostMeal(mealDto);

            _repository.Should().NotBeEmpty().And.HaveCount(2);
        }
    }
}
