using FluentAssertions;
using Mealmap.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace Mealmap.Api.NarrowIntegrationTests
{
    [Trait("Target", "Pipeline")]
    public class MealsPipelineTests
    {
        [Fact]
        public async void GetMeals_WhenMealsFound_ReturnsJsonAndStatusOk()
        {
            var factory = new MockableWebApplicationFactory(services =>
            {
                services.Replace(ServiceDescriptor.Scoped<IMealRepository>(_ =>
                {
                    List<Meal> meals = new() { new Meal() { Id = Guid.NewGuid(), DiningDate = new DateOnly(2020, 1, 1) } };
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
                    Meal meal = new() { Id = Guid.NewGuid() };
                    return Mock.Of<IMealRepository>(mock => mock.GetById(It.IsAny<Guid>()) == meal);
                }));
            });

            var response = await factory.CreateClient().GetAsync("/api/meals/" + Guid.NewGuid());

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
        }
    }
}
