﻿using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.DishAggregate;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SystemHeaders = System.Net.Http.Headers;

namespace Mealmap.Api.BoundaryTests;

public class DishesPipelineTests
{
    private readonly Dish _dummyDish;

    public DishesPipelineTests()
    {
        _dummyDish = new("Tuna Supreme") { Description = null, Servings = 2 };
    }


    [Fact]
    public async void GetDishes_ReturnsJsonAndStatusOk()
    {
        var factory = new MockableWebApplicationFactory(services =>
        {
            services.Replace(ServiceDescriptor.Scoped<IDishRepository>(_ =>
            {
                return Mock.Of<IDishRepository>(mock => mock.GetAll() == new List<Dish> { _dummyDish });
            }));
        });

        var response = await factory.CreateClient().GetAsync("/api/dishes");

        response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async void GetDish_ReturnsJsonAndStatusOk()
    {
        var factory = new MockableWebApplicationFactory(services =>
        {
            services.Replace(ServiceDescriptor.Scoped<IDishRepository>(_ =>
            {
                return Mock.Of<IDishRepository>(mock => mock.GetSingleById(It.IsAny<Guid>()) == _dummyDish);
            }));
        });

        var response = await factory.CreateClient().GetAsync("/api/dishes/" + _dummyDish.Id.ToString());

        response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async void PutDishImage_WhenImageUploaded_ReturnsNoBodyAndContentType()
    {
        var factory = new MockableWebApplicationFactory(services =>
        {
            services.Replace(ServiceDescriptor.Scoped<IDishRepository>(_ =>
            {
                return Mock.Of<IDishRepository>(mock => mock.GetSingleById(It.IsAny<Guid>()) == _dummyDish);
            }));
            services.Replace(ServiceDescriptor.Scoped<IUnitOfWork>(_ => Mock.Of<IUnitOfWork>()));
        });

        var content = new ByteArrayContent(new byte[1]);
        content.Headers.ContentType = SystemHeaders.MediaTypeHeaderValue.Parse("image/jpeg");
        var response = await factory.CreateClient().PutAsync("/api/dishes/" + _dummyDish.Id.ToString() + "/image", content);

        response.Content.Headers.ContentLength.Should().Be(0);
        response.Content.Headers.ContentType.Should().BeNull();
    }

    [Fact]
    public async void PutDishImage_WhenFileNotSupportedImageType_ReturnsUnsupportedMediaType()
    {
        var factory = new MockableWebApplicationFactory(services =>
        {
            services.Replace(ServiceDescriptor.Scoped<IDishRepository>(_ =>
            {
                return Mock.Of<IDishRepository>();
            }));
        });

        var content = new ByteArrayContent(new byte[1]);
        content.Headers.ContentType = SystemHeaders.MediaTypeHeaderValue.Parse("application/json");
        var response = await factory.CreateClient().PutAsync("/api/dishes/" + _dummyDish.Id.ToString() + "/image", content);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.UnsupportedMediaType);
    }

    [Fact]
    public async void GetDishImage_ReturnsCorrectContentTypeAndStatusOk()
    {
        const string contentType = "image/jpeg";
        var factory = new MockableWebApplicationFactory(services =>
        {
            services.Replace(ServiceDescriptor.Scoped<IDishRepository>(_ =>
            {
                _dummyDish.SetImage(new byte[1], contentType);
                return Mock.Of<IDishRepository>(m => m.GetSingleById(It.IsAny<Guid>()) == _dummyDish);
            }));
        });

        var response = await factory.CreateClient().GetAsync("/api/dishes/" + _dummyDish.Id.ToString() + "/image");

        response.Content.Headers.ContentType!.MediaType.Should().Be(contentType);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async void GetDishImage_WhenDishHasNoImage_ReturnsNoContent()
    {
        var factory = new MockableWebApplicationFactory(services =>
        {
            services.Replace(ServiceDescriptor.Scoped<IDishRepository>(_ =>
            {
                return Mock.Of<IDishRepository>(m => m.GetSingleById(It.IsAny<Guid>()) == _dummyDish);
            }));
        });

        var response = await factory.CreateClient().GetAsync("/api/dishes/" + _dummyDish.Id.ToString() + "/image");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
    }

    [Fact]
    public async void GetDishImage_WhenDishDoesntExist_ReturnsNotFound()
    {
        var factory = new MockableWebApplicationFactory(services =>
        {
            services.Replace(ServiceDescriptor.Scoped<IDishRepository>(_ =>
            {
                return Mock.Of<IDishRepository>(m => m.GetSingleById(It.IsAny<Guid>()) == null);
            }));
        });

        var response = await factory.CreateClient().GetAsync("/api/dishes/" + Guid.NewGuid() + "/image");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }
}
