using FluentAssertions;
using Mealmap.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using SystemHeaders = System.Net.Http.Headers;

namespace Mealmap.Api.NarrowIntegrationTests
{
    public class DishesPipelineTests
    {
        [Fact]
        public async void GetDishes_ReturnsJsonAndStatusOk()
        {
            Guid guid = Guid.NewGuid();
            var factory = new MockableWebApplicationFactory(services =>
            {
                services.Replace(ServiceDescriptor.Scoped<IDishRepository>(_ =>
                {
                    Dish dish = new("Tuna Supreme") { Id = guid };
                    return Mock.Of<IDishRepository>(mock => mock.GetAll() == new List<Dish> { dish });
                }));
            });

            var response = await factory.CreateClient().GetAsync("/api/dishes");

            response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
            response.Should().BeSuccessful();
        }

        [Fact]
        public async void PutDishImage_WhenImageUploaded_ReturnsNoBodyAndContentType()
        {
            Guid guid = Guid.NewGuid();
            var factory = new MockableWebApplicationFactory(services =>
            {
                services.Replace(ServiceDescriptor.Scoped<IDishRepository>(_ =>
                {
                    Dish dish = new("Tuna Supreme") { Id =  guid };
                    return Mock.Of<IDishRepository>(mock => mock.GetById(It.IsAny<Guid>()) == dish);
                }));
            });

            var content = new ByteArrayContent(new byte[1]);
            content.Headers.ContentType = SystemHeaders.MediaTypeHeaderValue.Parse("image/jpeg");
            var response = await factory.CreateClient().PutAsync("/api/dishes/" + guid.ToString() + "/image", content);

            response.Content.Headers.ContentLength.Should().Be(0);
            response.Content.Headers.ContentType.Should().BeNull();
        }
    }
}
