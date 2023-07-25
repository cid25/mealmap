using AutoMapper;
using FluentAssertions;
using Mealmap.Api.Controllers;
using Mealmap.Api.DataTransfer;
using Mealmap.Api.Formatters;
using Mealmap.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            var contextMock = Mock.Of<IRequestContext>(m => m.Scheme == "https" && m.Host == "test.com" && m.Port == 443);
            _controller = new DishesController(
                _logger,
                _repository,
                new DishMapper(
                    Mock.Of<ILogger<DishMapper>>(),
                    new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>()).CreateMapper(),
                    contextMock
                ),
                contextMock
            );

            const string someGuid = "00000000-0000-0000-0000-000000000001";
            var dishWithoutImage = new Dish("Krabby Patty") { Id = new Guid(someGuid) };
            _repository.Add(dishWithoutImage);

            const string anotherGuid = "00000000-0000-0000-0000-000000000002";
            var dishWithImage = new Dish("Tuna Supreme")
            {
                Id = new Guid(anotherGuid),
                Image = new DishImage(content: new byte[1], contentType: "image/jpeg")
            };
            _repository.Add(dishWithImage);
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
        public void PostDish_WhenDishIsValid_ReturnsDTO()
        {
            const string someDishName = "Sailors Surprise";
            DishDTO dish = new(someDishName);

            var contextMock = Mock.Of<IRequestContext>(m => m.Method == "POST");
            var mapperMock = Mock.Of<IDishMapper>(m =>
                m.MapFromDTO(It.IsAny<DishDTO>()) == new Dish(someDishName) &&
                m.MapFromEntity(It.IsAny<Dish>()) == dish);
            var controller = new DishesController(_logger, _repository, mapperMock, contextMock);

            var result = controller.PostDish(dish);

            result.Result.Should().BeOfType<CreatedAtActionResult>();
            ((CreatedAtActionResult)result.Result!).Value.Should().BeOfType<DishDTO>();
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
        public void PostDish_WhenMapperThrowsException_ReturnsBadRequest()
        {
            var mapperMock = new Mock<IDishMapper>();
            mapperMock.Setup(m => m.MapFromDTO(It.IsAny<DishDTO>())).Throws(new InvalidOperationException());
            var controller = new DishesController(_logger, _repository, mapperMock.Object, Mock.Of<IRequestContext>());

            const string someDishName = "Sailors Surprise";
            var someGuid = Guid.NewGuid();
            DishDTO dish = new(someDishName) { Id = someGuid };

            var result = controller.PostDish(dish);

            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void PutDish_SavesUpdateAndReturnsDTO()
        {
            const string someDishName = "Sailors Surprise";
            Guid nonExistingGuid = Guid.NewGuid();
            var eTag = "AAAA";
            DishDTO dish = new(someDishName)
            {
                Id = nonExistingGuid,
                ETag = eTag
            };
            var mockRepository = new Mock<IDishRepository>();
            mockRepository.Setup(m => m.GetSingle(It.IsAny<Guid>())).Returns(new Dish(someDishName) { Version = Convert.FromBase64String(eTag) });
            mockRepository.Setup(m => m.Update(It.IsAny<Dish>(), true)).Throws(new DbUpdateConcurrencyException());
            var controller = new DishesController(_logger, mockRepository.Object,
                Mock.Of<IDishMapper>(), Mock.Of<IRequestContext>(m => m.IfMatchHeader == eTag));

            var result = controller.PutDish(dish);

            mockRepository.Verify(m => m.Update(It.IsAny<Dish>(), true), Times.Once);
            result.Should().BeOfType<ActionResult<DishDTO>>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void PutDish_WhenIfMatchHeaderNotSet_ReturnsPreconditionRequired(string? header)
        {
            var controller = new DishesController(_logger, _repository, Mock.Of<IDishMapper>(), Mock.Of<IRequestContext>(m => m.IfMatchHeader == header));

            const string someDishName = "Sailors Surprise";
            var someGuid = Guid.NewGuid();
            DishDTO dish = new(someDishName) { Id = someGuid };

            var result = controller.PutDish(dish);

            result.Result.Should().BeOfType<StatusCodeResult>();
            ((StatusCodeResult)result.Result!).StatusCode.Should().Be(428);
        }

        [Fact]
        public void PutDish_WhenIdNotSet_ReturnsBadRequest()
        {
            const string someHeader = "AAAA";
            var controller = new DishesController(_logger, _repository, Mock.Of<IDishMapper>(), Mock.Of<IRequestContext>(m => m.IfMatchHeader == someHeader));

            const string someDishName = "Sailors Surprise";
            DishDTO dish = new(someDishName);

            var result = controller.PutDish(dish);

            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void PutDish_WhenDishDoesntExist_ReturnsNotFound()
        {
            const string someETag = "AAAA";
            var controller = new DishesController(_logger, _repository, Mock.Of<IDishMapper>(), Mock.Of<IRequestContext>(m => m.IfMatchHeader == someETag));

            const string someDishName = "Sailors Surprise";
            Guid nonExistingGuid = Guid.NewGuid();
            DishDTO dish = new(someDishName) { Id = nonExistingGuid };

            var result = controller.PutDish(dish);

            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void PutDish_WhenSavingThrowsConcurrencyException_ReturnsPreconditionFailed()
        {
            const string someDishName = "Sailors Surprise";
            Guid nonExistingGuid = Guid.NewGuid();
            var eTag = "AAAA";
            DishDTO dish = new(someDishName)
            {
                Id = nonExistingGuid,
                ETag = eTag
            };
            var mockRepository = new Mock<IDishRepository>();
            mockRepository.Setup(m => m.GetSingle(It.IsAny<Guid>())).Returns(new Dish(someDishName) { Version = Convert.FromBase64String(eTag) });
            mockRepository.Setup(m => m.Update(It.IsAny<Dish>(), true)).Throws(new DbUpdateConcurrencyException());
            var controller = new DishesController(_logger, mockRepository.Object,
                Mock.Of<IDishMapper>(), Mock.Of<IRequestContext>(m => m.IfMatchHeader == eTag));

            var result = controller.PutDish(dish);

            result.Result.Should().BeOfType<StatusCodeResult>();
            ((StatusCodeResult)result.Result!).StatusCode.Should().Be(412);
        }

        [Fact]
        public void DeleteDish_WhenDishExists_ReturnsOkAndDish()
        {
            var dish = _repository.GetAll().First();

            var result = _controller.DeleteDish(dish.Id);

            result.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result.Result!).Value.Should().BeOfType<DishDTO>();
        }

        [Fact]
        public void DeleteDish_WhenDishDoesntExist_ReturnsNotFound()
        {
            Guid nonExistingDishGuid = new Guid("99999999-9999-9999-9999-999999999999");

            var result = _controller.DeleteDish(nonExistingDishGuid);

            result.Result.Should().BeOfType<NotFoundObjectResult>();
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

        [Fact]
        public void DeleteDishImage_ReturnsOk()
        {
            Guid dishWithImage = new("00000000-0000-0000-0000-000000000002");

            var result = _controller.DeleteDishImage(dishWithImage);

            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public void DeleteDishImage_WhenDishDoesntExist_ReturnsBadRequest()
        {
            Guid nonExistingGuid = new("99999999-9999-9999-9999-999999999999");

            var result = _controller.DeleteDishImage(nonExistingGuid);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void DeleteDishImage_WhenNoImage_ReturnsNoContent()
        {
            Guid dishWithoutImage = new("00000000-0000-0000-0000-000000000001");

            var result = _controller.DeleteDishImage(dishWithoutImage);

            result.Should().BeOfType<NoContentResult>();
        }
    }
}
