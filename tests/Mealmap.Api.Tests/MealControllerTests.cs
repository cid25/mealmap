using FluentAssertions;
using Mealmap.Api.Controllers;
using Mealmap.Api.DataTransferObjects;
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

            result.Should().BeOfType<ActionResult<MealDto>>();
            result.Value.Should().NotBeNull();
            result.Value!.Name.Should().Be("Cheeseburger");
        }
    }
}
