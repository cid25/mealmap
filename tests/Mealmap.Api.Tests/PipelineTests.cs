using FluentAssertions;
using Mealmap.Api.UnitTests;
using Mealmap.Model;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mealmap.Api.PipelineTests
{
    public class PipelineTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public PipelineTests(WebApplicationFactory<Program> applicationFactory)
        {
            _client = applicationFactory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services
                    .RemoveAll<IMealRepository>()
                    .AddScoped<IMealRepository, FakeMealRepository>()
                    .RemoveAll<IDishRepository>()
                    .AddScoped<IDishRepository, FakeDishRepository>();
                });
            })
            .CreateClient();
        }


        [Fact]
        public async void Meals_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/meals");

            response.Should().BeSuccessful();
        }

        [Fact]
        public async void Meals_ReturnsJson()
        {
            var response = await _client.GetAsync("/api/meals");

            response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
        }

        [Fact]
        public async void Dishes_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/dishes");

            response.Should().BeSuccessful();
        }

        [Fact]
        public async void Dishes_ReturnsJson()
        {
            var response = await _client.GetAsync("/api/dishes");

            response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
        }
    }
}
