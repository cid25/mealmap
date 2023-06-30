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

        public MealControllerTests()
        {
            _repository = new FakeMealRepository();

            const string firstGuid = "00000000-0000-0000-0000-000000000001";
            var cheeseburger = new Meal(id: new Guid(firstGuid), name: "Cheeseburger");
            _repository.Create(cheeseburger);
        }

        [Fact]
        public void GetMeal_WhenGivenValidId_ReturnsMeal()
        {
            MealController mealController = new(repository: _repository);

            Guid guid = _repository.ElementAt(0).Key;
            var result = mealController.GetMeal(guid);

            result.Should().BeOfType<ActionResult<MealDto>>();
            result.Value.Should().NotBeNull();
            result.Value!.Id.Should().NotBeEmpty();
        }

        [Fact]
        public void GetMeal_WhenGivenNonExistingId_ReturnsNotFound()
        {
            MealController mealController = new(repository: _repository);

            const string nonExistingGuid = "99999999-9999-9999-9999-999999999999";
            var result = mealController.GetMeal(new Guid(nonExistingGuid));

            result.Result.Should().BeOfType<NotFoundResult>();
        }
    }
}
