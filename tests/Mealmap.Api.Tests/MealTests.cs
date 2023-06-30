using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;
using System.Diagnostics.CodeAnalysis;

namespace Mealmap.Api.Tests
{
    public class MealTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _applicationFactory;

        public MealTests(WebApplicationFactory<Program> applicationFactory)
        {
            _applicationFactory = applicationFactory;
        }


        [Fact]
        public async void Meal_ReturnsOk()
        {
            var client = _applicationFactory.CreateClient();

            var response = await client.GetAsync("/Meal");

            response.Should().BeSuccessful();
        }

        [Fact]
        public async void Meal_ReturnsJson()
        {
            var client = _applicationFactory.CreateClient();

            var response = await client.GetAsync("/Meal");

            response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
        }
    }
}
