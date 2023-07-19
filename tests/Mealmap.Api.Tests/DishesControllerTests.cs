using AutoMapper;
using FluentAssertions;
using Mealmap.Api.Controllers;
using Mealmap.Api.DataTransfer;
using Mealmap.Api.Formatters;
using Mealmap.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Mealmap.Api.UnitTests
{
    public class DishesControllerTests
    {
        private readonly ILogger<DishesController> _logger;
        private readonly FakeDishRepository _repository;
        private readonly DishesController _controller;

        public DishesControllerTests()
        {
            _logger = (new Mock<ILogger<DishesController>>()).Object;
            _repository = new FakeDishRepository();

            _controller = new DishesController(
                _logger,
                _repository,
                new DishMapper(
                    new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>()).CreateMapper(),
                    Mock.Of<IHttpContextAccessor>(accessor =>
                        accessor.HttpContext == Mock.Of<HttpContext>(context =>
                            context.Request == Mock.Of<HttpRequest>(request =>
                                request.Scheme == "https" && request.Host == new HostString("test.com", 443))))));

            const string someGuid = "00000000-0000-0000-0000-000000000001";
            var dishWithoutImage = new Dish("Krabby Patty") { Id = new Guid(someGuid) };
            _repository.Create(dishWithoutImage);

            const string anotherGuid = "00000000-0000-0000-0000-000000000002";
            var dishWithImage = new Dish("Tuna Supreme")
            {
                Id = new Guid(anotherGuid),
                Image = new DishImage(content: new byte[1], contentType: "image/jpeg")
            };
            _repository.Create(dishWithImage);
        }

        [Fact]
        public void GetDishes_ReturnsDishDTOs()
        {
            var result = _controller.GetDishes();

            result.Should().BeOfType<ActionResult<IEnumerable<DishDTO>>>();
            result.Value.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public void GetDish_WhenDishExists_ReturnsDish()
        {
            var existingId = _repository.ElementAt(0).Key;

            var result = _controller.GetDish(existingId);

            result.Should().BeOfType<ActionResult<DishDTO>>();
            result.Value!.Id.Should().Be(existingId);
        }

        [Fact]
        public void GetDish_WhenDishWithIdDoesntExist_ReturnsNotFound()
        {
            const string nonExistingId = "99999999-9999-9999-9999-999999999999";
            var result = _controller.GetDish(new Guid(nonExistingId));

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
        public void PostDish_WhenDishHasEmptyName_ReturnsBadRequest(string name)
        {
            DishDTO dish = new(name: name);

            var result = _controller.PostDish(dish);

            result.Result.Should().BeOfType<BadRequestResult>();
        }

        [Theory]
        [InlineData("image/jpeg")]
        [InlineData("image/png")]
        public void PutDishImage_WhenImageIsProper_ReturnsStatusCodeCreated(string contentType)
        {
            var dishId = _repository.ElementAt(0).Key;
            var imageDummy = new Image(
                content: new byte[1],
                contentType: contentType
            );

            var result = _controller.PutDishImage(dishId, imageDummy);

            result.Should().BeOfType<StatusCodeResult>();
            ((StatusCodeResult)result).StatusCode.Should().Be(201);
        }

        [Fact]
        public void PutDishImage_WhenImageIsProper_UpdatesDish()
        {
            var idOfDishWithoutImage = _repository.ElementAt(0).Key;
            var imageDummy = new Image(
                content: new byte[1],
                contentType: "image/jpeg"
            );

            _controller.PutDishImage(idOfDishWithoutImage, imageDummy);

            _repository.TryGetValue(idOfDishWithoutImage, out var result);
            if (result == null)
                throw new ArgumentNullException(nameof(result));
            result.Image.Should().NotBeNull();
        }

        [Fact]
        public void GetDishImage_WhenImageExists_ReturnsFileContentResult()
        {
            Guid guidOfDishWithImage = new Guid("00000000-0000-0000-0000-000000000002");

            var result = _controller.GetDishImage(guidOfDishWithImage);

            result.Should().BeOfType<FileContentResult>();
        }

        [Fact]
        public void GetDishImage_WhenImageDoesntExist_ReturnsNoContent()
        {
            Guid guidOfDishWithoutImage = new Guid("00000000-0000-0000-0000-000000000001");

            var result = _controller.GetDishImage(guidOfDishWithoutImage);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public void GetDishImage_WhenDishDoesntExist_ReturnsNotFound()
        {
            Guid nonExistingDishGuid = new Guid("99999999-9999-9999-9999-999999999999");

            var result = _controller.GetDishImage(nonExistingDishGuid);

            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
