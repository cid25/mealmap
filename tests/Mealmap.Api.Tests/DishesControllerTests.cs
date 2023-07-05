using FluentAssertions;
using Mealmap.Api.Controllers;
using Mealmap.Api.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;

namespace Mealmap.Api.UnitTests
{
    public class DishesControllerTests
    {
        DishesController _controller;

        public DishesControllerTests()
        {
            _controller = new DishesController();
        }

        [Fact]
        public void GetDishes_ReturnsDishDTOs()
        {
            var result = _controller.GetDishes();

            result.Should().BeOfType<ActionResult<IEnumerable<DishDTO>>>();
        }
    }
}
