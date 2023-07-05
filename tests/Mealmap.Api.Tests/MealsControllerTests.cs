﻿using AutoMapper;
using FluentAssertions;
using Mealmap.Api.Controllers;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Model;
using Microsoft.AspNetCore.Mvc;

namespace Mealmap.Api.UnitTests
{
    public class MealsControllerTests
    {
        private readonly FakeMealRepository _repository;
        private readonly MealsController _controller;

        public MealsControllerTests()
        {
            _repository = new FakeMealRepository();
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<MealmapMapperProfile>());
            _controller = new MealsController(_repository, mapperConfig.CreateMapper());

            const string firstGuid = "00000000-0000-0000-0000-000000000001";
            var cheeseburger = new Meal() { Id = new Guid(firstGuid) };
            _repository.Create(cheeseburger);
        }

        [Fact]
        public void GetMeals_ReturnsMealDTOs()
        {
            var result = _controller.GetMeals();

            result.Should().BeOfType<ActionResult<IEnumerable<MealDTO>>>();
        }

        [Fact]
        public void GetMeal_WhenMealWithIdExisting_ReturnsMeal()
        {
            Guid guid = _repository.ElementAt(0).Key;
            var result = _controller.GetMeal(guid);

            result.Should().BeOfType<ActionResult<MealDTO>>();
            result.Value.Should().NotBeNull();
            result.Value!.Id.Should().NotBeEmpty();
        }

        [Fact]
        public void GetMeal_WhenMealWithIdNotExisting_ReturnsNotFound()
        {
            const string nonExistingGuid = "99999999-9999-9999-9999-999999999999";
            var result = _controller.GetMeal(new Guid(nonExistingGuid));

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void PostMeal_WhenGivenValidMeal_ReturnsMealWithId()
        {
            MealDTO mealDto = new();

            var result = _controller.PostMeal(mealDto);

            result.Value.Should().BeOfType<MealDTO>();
            result.Value!.Id.Should().NotBeNull().And.NotBeEmpty();
        }

        [Fact]
        public void PostMeal_WhenMealIsValid_StoresMeal()
        {;
            MealDTO mealDto = new();

            _ = _controller.PostMeal(mealDto);

            _repository.Should().NotBeEmpty().And.HaveCount(2);
        }

        [Fact]
        public void PostMeal_WhenMealAlreadyHasId_ReturnsBadRequest()
        {
            MealDTO mealDto = new() { Id = Guid.NewGuid() };

            var result = _controller.PostMeal(mealDto);

            result.Result.Should().BeOfType<BadRequestResult>();
        }
    }
}
