using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Mealmap.Api.Tests
{
    public class MealEndpointTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _applicationFactory;

        public MealEndpointTests(WebApplicationFactory<Program> applicationFactory)
        {
            _applicationFactory = applicationFactory;
        }


        [Fact]
        public async void Meal_ReturnsOk()
        {
            var client = _applicationFactory.CreateClient();

            var response = await client.GetAsync("/meal");

            response.Should().BeSuccessful();
        }

        [Fact]
        public async void Meal_ReturnsJson()
        {
            var client = _applicationFactory.CreateClient();

            var response = await client.GetAsync("/meal");

            response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
        }
    }
}
