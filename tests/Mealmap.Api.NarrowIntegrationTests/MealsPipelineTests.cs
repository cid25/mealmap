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
        public async void GetMeals_ReturnsJsonAndStatusOk()
        {
            Guid guid = Guid.NewGuid();
            var factory = new MockableWebApplicationFactory(services =>
            {
                services.Replace(ServiceDescriptor.Scoped<IMealRepository>(_ =>
                {
                    Meal meal = new() { Id = guid };
                    return Mock.Of<IMealRepository>(mock => mock.GetById(It.IsAny<Guid>()) == meal);
                }));
            });

            var response = await factory.CreateClient().GetAsync("/api/meals");

            response.Should().BeSuccessful();
            response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
        }
    }
}
