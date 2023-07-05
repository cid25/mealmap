﻿using FluentAssertions;
using Mealmap.Api.UnitTests;
using Mealmap.Model;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mealmap.Api.PipelineTests
{
    public class BasicApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public BasicApiTests(WebApplicationFactory<Program> applicationFactory)
        {
            _client = applicationFactory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<IMealRepository>().AddScoped<IMealRepository,FakeMealRepository>();
                });
            })
            .CreateClient();
        }


        [Fact]
        public async void Meal_ReturnsOk()
        {
            var response = await _client.GetAsync("/meals");

            response.Should().BeSuccessful();
        }

        [Fact]
        public async void Meal_ReturnsJson()
        {
            var response = await _client.GetAsync("/meals");

            response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
        }
    }
}
