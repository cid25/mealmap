using FluentAssertions;
using Mealmap.Controllers;
using Mealmap.Model;
using Microsoft.AspNetCore.Mvc;

namespace Mealmap.Api.Tests
{
    public class MealControllerTests
    {
        [Fact]
        public void Get_ReturnsMeal()
        {
            MealController mealController = new();

            var result = mealController.Get();

            result.Should().BeOfType<ActionResult<Meal>>();
            result.Value.Should().NotBeNull();
            result.Value!.Name.Should().Be("Cheeseburger");
        }
    }
}
