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
    public class DishesControllerTests
    {
        ILogger<DishesController> _logger;
        FakeDishRepository _repository;
        DishesController _controller;
        
        public DishesControllerTests()
        {
            _logger = (new Mock<ILogger<DishesController>>()).Object;
            _repository = new FakeDishRepository();
            var mapper = (new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>())).CreateMapper();

            _controller = new DishesController(_logger, _repository, mapper);

            const string someGuid = "00000000-0000-0000-0000-000000000001";
            _repository.Create(new Dish("Krabby Patty") { Id = new Guid(someGuid)} );
        }

        [Fact]
        public void GetDishes_ReturnsDishDTOs()
        {
            var result = _controller.GetDishes();

            result.Should().BeOfType<ActionResult<IEnumerable<DishDTO>>>();
            result.Value.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public void GetDish_WhenDishWithIdExisting_ReturnsDish()
        {
            const string existingGuid = "00000000-0000-0000-0000-000000000001";
            var result = _controller.GetDish(new Guid(existingGuid));

            result.Should().BeOfType<ActionResult<DishDTO>>();
            result.Value!.Id.Should().Be(existingGuid);
        }
        
        [Fact]
        public void GetDish_WhenDishWithIdNotExisting_ReturnsNotFound()
        {
            const string nonExistingGuid = "99999999-9999-9999-9999-999999999999";
            var result = _controller.GetDish(new Guid(nonExistingGuid));

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void PostDish_WhenDishIsValid_ReturnsDishWithId()
        {
            const string someDishName = "Sailors Surprise";
            DishDTO dish = new(someDishName);

            var result = _controller.PostDish(dish);

            result.Result.Should().BeOfType<CreatedAtActionResult>();
            ((CreatedAtActionResult)result.Result!).Value.Should().BeOfType<DishDTO>();
            var value = (DishDTO)((CreatedAtActionResult)result.Result!).Value!;
            value.Id.Should().NotBeNull().And.NotBeEmpty();
        }

        [Fact]
        public void PostDish_WhenDishIsValid_StoresDish()
        {
            const string someDishName = "Sailors Surprise";
            DishDTO dish = new(someDishName);

            _ = _controller.PostDish(dish);

            _repository.Should().NotBeEmpty().And.HaveCountGreaterThan(1);
        }

        [Fact]
        public void PostDish_WhenDishAlreadyHasId_ReplacesId()
        {
            const string someDishName = "Sailors Surprise";
            var someGuid = Guid.NewGuid();
            DishDTO dish = new(someDishName) { Id = someGuid };

            var result = _controller.PostDish(dish);

            var value = (DishDTO)((CreatedAtActionResult)result.Result!).Value!;
            value.Id.Should().NotBeNull().And.NotBeEmpty().And.NotBe(someGuid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void PostDish_WhenGivenDishWithEmptyName_ReturnsBadRequest(string name)
        {
            DishDTO dish = new(name: name);

            var result = _controller.PostDish(dish);

            result.Result.Should().BeOfType<BadRequestResult>();
        }
    }
}
