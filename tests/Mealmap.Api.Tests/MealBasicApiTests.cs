using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Mealmap.Api.UnitTests
{
    public class MealBasicApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _applicationFactory;

        public MealBasicApiTests(WebApplicationFactory<Program> applicationFactory)
        {
            _applicationFactory = applicationFactory;
        }


        [Fact]
        public async void Meal_ReturnsOk()
        {
            var client = _applicationFactory.CreateClient();

            var response = await client.GetAsync("/meals");

            response.Should().BeSuccessful();
        }

        [Fact]
        public async void Meal_ReturnsJson()
        {
            var client = _applicationFactory.CreateClient();

            var response = await client.GetAsync("/meals");

            response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
        }
    }
}
