﻿using AutoMapper;
using Mealmap.Api.Commands;
using Mealmap.Api.Controllers;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Api.OutputMappers;
using Mealmap.Api.RequestFormatters;
using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.DishAggregate;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Mealmap.Api.UnitTests.Controllers;

public class DishesControllerTests
{
    private readonly ILogger<DishesController> _loggerMock = new Mock<ILogger<DishesController>>().Object;
    private readonly FakeDishRepository _repositoryFake = new();
    private readonly DishesController _controller;
    private readonly Dish[] _dishes;

    public DishesControllerTests()
    {
        var contextMock = Mock.Of<IRequestContext>(m => m.Scheme == "https" && m.Host == "test.com" && m.Port == 443);
        var baseMapper = new MapperConfiguration(cfg => cfg.AddProfile<AutomapperProfile>()).CreateMapper();
        _controller = new DishesController(
            _loggerMock,
            _repositoryFake,
            Mock.Of<IUnitOfWork>(),
            new DishOutputMapper(baseMapper, contextMock),
            contextMock,
            Mock.Of<IMediator>()
        );

        _dishes = new Dish[2];
        seedData();
    }

    private void seedData()
    {
        Dish dishWithoutImage = new("Krabby Patty", null, 2);
        _dishes[0] = dishWithoutImage;
        _repositoryFake.Add(dishWithoutImage);

        Dish dishWithImage = new("Tuna Supreme", null, 2);
        dishWithImage.SetImage(new byte[1], "image/jpeg");
        _dishes[1] = dishWithImage;
        _repositoryFake.Add(dishWithImage);
    }

    [Fact]
    public async void GetDishes_ReturnsDishDTOs()
    {
        var result = await _controller.GetDishes(next: null, limit: null);

        result.Should().BeOfType<ActionResult<PaginatedDTO<DishDTO>>>();
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public void GetDish_WhenDishExists_ReturnsDish()
    {
        var existingId = _repositoryFake.ElementAt(0).Key;

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
    public async void PostDish_WhenDishIsValid_ReturnsMeal()
    {
        DishDTO dto = new("fakeName");
        CommandNotification<DishDTO> notification = new() { Result = dto };
        var mediatorMock = new Mock<IMediator>();
        mediatorMock.Setup(m => m.Send(It.IsAny<CreateDishCommand>(), It.IsAny<CancellationToken>()).Result)
            .Returns(notification);
        var controller = new DishesController(
            Mock.Of<ILogger<DishesController>>(),
            _repositoryFake,
            Mock.Of<IUnitOfWork>(),
            Mock.Of<IOutputMapper<DishDTO, Dish>>(),
            Mock.Of<IRequestContext>(),
            mediatorMock.Object
        );

        var result = await controller.PostDish(dto);

        result.Result.Should().BeOfType<CreatedAtActionResult>();
        ((CreatedAtActionResult)result.Result!).Value.Should().BeOfType<DishDTO>();
        var value = (DishDTO)((CreatedAtActionResult)result.Result!).Value!;
    }

    [Fact]
    public async void PostDish_WhenDishIsInvalid_ReturnsBadRequest()
    {
        DishDTO dto = new("fakeName");
        CommandNotification<DishDTO> notification = new();
        notification.Errors.Add(new CommandError(CommandErrorCodes.NotValid));
        var mediatorMock = new Mock<IMediator>();
        mediatorMock.Setup(m => m.Send(It.IsAny<CreateDishCommand>(), It.IsAny<CancellationToken>()).Result)
            .Returns(notification);
        var controller = new DishesController(
            Mock.Of<ILogger<DishesController>>(),
            _repositoryFake,
            Mock.Of<IUnitOfWork>(),
            Mock.Of<IOutputMapper<DishDTO, Dish>>(),
            Mock.Of<IRequestContext>(),
            mediatorMock.Object
        );

        var result = await controller.PostDish(dto);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async void PutDish_WhenIfMatchHeaderNotSet_ReturnsPreconditionRequired(string? header)
    {
        var controller = new DishesController(
            _loggerMock,
            _repositoryFake,
            Mock.Of<IUnitOfWork>(),
            Mock.Of<IOutputMapper<DishDTO, Dish>>(),
            Mock.Of<IRequestContext>(m => m.IfMatchHeader == header),
            Mock.Of<IMediator>()
        );

        DishDTO dish = new("dummyName");
        var result = await controller.PutDish(Guid.NewGuid(), dish);

        result.Result.Should().BeOfType<StatusCodeResult>();
        ((StatusCodeResult)result.Result!).StatusCode.Should().Be(428);
    }

    [Fact]
    public async void PutDish_WhenVersionDoesNotMatch_ReturnsPreconditionFailed()
    {
        CommandNotification<DishDTO> notification = new();
        notification.Errors.Add(new CommandError(CommandErrorCodes.VersionMismatch));
        var mediatorMock = new Mock<IMediator>();
        mediatorMock.Setup(m => m.Send(It.IsAny<UpdateDishCommand>(), It.IsAny<CancellationToken>()).Result)
            .Returns(notification);
        var controller = new DishesController(
            _loggerMock,
            _repositoryFake,
            Mock.Of<IUnitOfWork>(),
            Mock.Of<IOutputMapper<DishDTO, Dish>>(),
            Mock.Of<IRequestContext>(m => m.IfMatchHeader == "fakeVersion"),
            mediatorMock.Object
        );

        DishDTO dummyDish = new("dummyName");
        var result = await controller.PutDish(Guid.NewGuid(), dummyDish);

        result.Result.Should().BeOfType<StatusCodeResult>();
        ((StatusCodeResult)result.Result!).StatusCode.Should().Be(412);
    }

    [Fact]
    public async void PutDish_WhenDishNotFound_ReturnsNotFound()
    {
        CommandNotification<DishDTO> notification = new();
        notification.Errors.Add(new CommandError(CommandErrorCodes.NotFound));
        var mediatorMock = new Mock<IMediator>();
        mediatorMock.Setup(m => m.Send(It.IsAny<UpdateDishCommand>(), It.IsAny<CancellationToken>()).Result)
            .Returns(notification);
        var controller = new DishesController(
            _loggerMock,
            _repositoryFake,
            Mock.Of<IUnitOfWork>(),
            Mock.Of<IOutputMapper<DishDTO, Dish>>(),
            Mock.Of<IRequestContext>(m => m.IfMatchHeader == "fakeVersion"),
            mediatorMock.Object
        );

        DishDTO dummyDish = new("dummyName");
        var result = await controller.PutDish(Guid.NewGuid(), dummyDish);

        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async void PutDish_WhenValidationError_ReturnsBadRequest()
    {
        CommandNotification<DishDTO> notification = new();
        notification.Errors.Add(new CommandError(CommandErrorCodes.NotValid));
        var mediatorMock = new Mock<IMediator>();
        mediatorMock.Setup(m => m.Send(It.IsAny<UpdateDishCommand>(), It.IsAny<CancellationToken>()).Result)
            .Returns(notification);
        var controller = new DishesController(
            _loggerMock,
            _repositoryFake,
            Mock.Of<IUnitOfWork>(),
            Mock.Of<IOutputMapper<DishDTO, Dish>>(),
            Mock.Of<IRequestContext>(m => m.IfMatchHeader == "fakeVersion"),
            mediatorMock.Object
        );

        DishDTO dummyDish = new("dummyName");
        var result = await controller.PutDish(Guid.NewGuid(), dummyDish);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async void DeleteDish_WhenDishExists_ReturnsOkAndDish()
    {
        var dish = _repositoryFake.GetAll().First();

        var result = await _controller.DeleteDish(dish.Id);

        result.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)result.Result!).Value.Should().BeOfType<DishDTO>();
    }

    [Fact]
    public async void DeleteDish_WhenDishDoesntExist_ReturnsNotFound()
    {
        var nonExistingDishGuid = new Guid("99999999-9999-9999-9999-999999999999");

        var result = await _controller.DeleteDish(nonExistingDishGuid);

        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Theory]
    [InlineData("image/jpeg")]
    [InlineData("image/png")]
    public async void PutDishImage_WhenImageIsProper_ReturnsStatusCodeCreated(string contentType)
    {
        var dishId = _repositoryFake.ElementAt(0).Key;
        var imageDummy = new Image(
            content: new byte[1],
            contentType: contentType
        );

        var result = await _controller.PutDishImage(dishId, imageDummy);

        result.Should().BeOfType<StatusCodeResult>();
        ((StatusCodeResult)result).StatusCode.Should().Be(201);
    }

    [Fact]
    public async void PutDishImage_WhenImageIsProper_UpdatesDish()
    {
        var idOfDishWithoutImage = _repositoryFake.ElementAt(0).Key;
        var imageDummy = new Image(
            content: new byte[1],
            contentType: "image/jpeg"
        );

        await _controller.PutDishImage(idOfDishWithoutImage, imageDummy);

        _repositoryFake.TryGetValue(idOfDishWithoutImage, out var result);
        if (result == null)
            throw new ArgumentNullException(nameof(result));
        result.Image.Should().NotBeNull();
    }

    [Fact]
    public void GetDishImage_WhenImageExists_ReturnsFileContentResult()
    {
        Guid dishWithImage = _dishes[1].Id;

        var result = _controller.GetDishImage(dishWithImage);

        result.Should().BeOfType<FileContentResult>();
    }

    [Fact]
    public void GetDishImage_WhenImageDoesntExist_ReturnsNoContent()
    {
        Guid dishWithoutImage = _dishes[0].Id;

        var result = _controller.GetDishImage(dishWithoutImage);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public void GetDishImage_WhenDishDoesntExist_ReturnsNotFound()
    {
        var nonExistingDishGuid = new Guid("99999999-9999-9999-9999-999999999999");

        var result = _controller.GetDishImage(nonExistingDishGuid);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async void DeleteDishImage_ReturnsOk()
    {
        Guid dishWithImage = _dishes[1].Id;

        var result = await _controller.DeleteDishImage(dishWithImage);

        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async void DeleteDishImage_WhenDishDoesntExist_ReturnsBadRequest()
    {
        Guid nonExistingGuid = new("99999999-9999-9999-9999-999999999999");

        var result = await _controller.DeleteDishImage(nonExistingGuid);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async void DeleteDishImage_WhenNoImage_ReturnsNoContent()
    {
        Guid dishWithoutImage = _dishes[0].Id;

        var result = await _controller.DeleteDishImage(dishWithoutImage);

        result.Should().BeOfType<NoContentResult>();
    }
}
