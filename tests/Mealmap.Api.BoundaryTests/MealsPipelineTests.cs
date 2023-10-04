using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.MealAggregate;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mealmap.Api.BoundaryTests;

[Trait("Target", "Pipeline")]
public class MealsPipelineTests
{
    private readonly Meal _dummyMeal;

    public MealsPipelineTests()
    {
        _dummyMeal = new(DateOnly.FromDateTime(DateTime.Now));
    }

    [Fact]
    public async void GetMeals_WhenMealsFound_ReturnsJsonAndStatusOk()
    {
        var factory = new MockableWebApplicationFactory(services =>
        {
            services.Replace(ServiceDescriptor.Scoped<IMealRepository>(_ =>
            {
                List<Meal> meals = new() { _dummyMeal };
                return Mock.Of<IMealRepository>(mock => mock.GetAll(null, null) == meals);
            }));
        });

        var response = await factory.CreateClient().GetAsync("/api/meals");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async void GetMeals_WhenNoMealsFound_ReturnsNoContent()
    {
        var factory = new MockableWebApplicationFactory(services =>
        {
            services.Replace(ServiceDescriptor.Scoped<IMealRepository>(_ =>
            {
                List<Meal> meals = new();
                return Mock.Of<IMealRepository>(mock => mock.GetAll(It.IsAny<DateOnly>(), It.IsAny<DateOnly>()) == meals);
            }));
        });

        var response = await factory.CreateClient().GetAsync("/api/meals");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
    }

    [Fact]
    public async void GetMeal_ReturnsJsonAndStatusOk()
    {
        var factory = new MockableWebApplicationFactory(services =>
        {
            services.Replace(ServiceDescriptor.Scoped<IMealRepository>(_ =>
            {
                return Mock.Of<IMealRepository>(mock => mock.GetSingleById(It.IsAny<Guid>()) == _dummyMeal);
            }));
        });

        var response = await factory.CreateClient().GetAsync("/api/meals/" + _dummyMeal.Id.ToString());

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async void DeleteMeal_DeletesAndReturnsJsonAndStatusOk()
    {
        var repositoryMock = new Mock<IMealRepository>();
        repositoryMock.Setup(m => m.GetSingleById(It.IsAny<Guid>())).Returns(_dummyMeal);
        var factory = new MockableWebApplicationFactory(services =>
        {
            services.Replace(ServiceDescriptor.Scoped<IMealRepository>(_ =>
            {
                return repositoryMock.Object;
            }));
            services.Replace(ServiceDescriptor.Scoped<IUnitOfWork>(_ => Mock.Of<IUnitOfWork>()));
        });

        var response = await factory.CreateClient().DeleteAsync("/api/meals/" + Guid.NewGuid());

        repositoryMock.Verify(r => r.Remove(_dummyMeal), Times.Once());
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async void DeleteMeal_WhenMealDoesntExist_ReturnsNotFound()
    {
        var factory = new MockableWebApplicationFactory(services =>
        {
            services.Replace(ServiceDescriptor.Scoped<IMealRepository>(_ =>
            {
                return Mock.Of<IMealRepository>(mock => mock.GetSingleById(It.IsAny<Guid>()) == null);
            }));
        });

        var response = await factory.CreateClient().DeleteAsync("/api/meals/" + Guid.NewGuid());

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }
}
