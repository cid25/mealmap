using AutoMapper;
using FluentAssertions;
using Mealmap.Api.DataTransfer;
using Mealmap.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace Mealmap.Api.UnitTests
{
    public class DishMapperTests
    {
        private readonly DishMapper _dishMapper;

        public DishMapperTests()
        {
            var basicMapper = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>()).CreateMapper();

            _dishMapper = new DishMapper(
                basicMapper,
                Mock.Of<IHttpContextAccessor>(m => 
                    m.HttpContext == Mock.Of<HttpContext>(c => 
                        c.Request  == Mock.Of<HttpRequest>(r =>
                            r.Scheme == "https" && r.Host == new HostString("test.com:5000")))));
        }

        [Fact]
        public void MapFromEntity_ReturnsDto()
        {
            var guid = Guid.NewGuid();
            var name = "Sailors Suprise";
            var dish = new Dish(name)
            {
                Id = guid,
                Image = new DishImage(content: new byte[1], contentType: "image/jpeg")
            };

            var dto = _dishMapper.MapFromEntity(dish);

            dto.Id.Should().Be(guid);
            dto.Name.Should().Be(name);
            dto.ImageUrl!.ToString().Should().EndWith(guid.ToString() + "/image");
        }
    }
}
