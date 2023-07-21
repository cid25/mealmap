using AutoMapper;
using FluentAssertions;
using Mealmap.Api.DataTransfer;
using Mealmap.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace Mealmap.Api.UnitTests
{
    public class DishMapperTests
    {
        private readonly DishMapper _mapper;

        public DishMapperTests()
        {
            var basicMapper = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>()).CreateMapper();

            _mapper = new DishMapper(
                Mock.Of<ILogger<DishMapper>>(),
                basicMapper,
                Mock.Of<IHttpContextAccessor>(m =>
                    m.HttpContext == Mock.Of<HttpContext>(c =>
                        c.Request == Mock.Of<HttpRequest>(r =>
                            r.Scheme == "https" && r.Host == new HostString("test.com:5000")))));
        }

        [Fact]
        public void MapFromEntity_ReturnsDto()
        {
            const string SomeName = "Sailors Suprise";
            var someGuid = Guid.NewGuid();
            var dish = new Dish(SomeName)
            {
                Id = someGuid,
                Image = new DishImage(content: new byte[1], contentType: "image/jpeg")
            };

            var dto = _mapper.MapFromEntity(dish);

            dto.Id.Should().Be(someGuid);
            dto.Name.Should().Be(SomeName);
            dto.ImageUrl!.ToString().Should().EndWith(someGuid.ToString() + "/image");
        }

        [Fact]
        public void MapFromDTO_WhenNameIsntSet_ThrowsException()
        {
            var someGuid = Guid.NewGuid();
            DishDTO dto = new(String.Empty);

            Action act = () => _mapper.MapFromDTO(dto);

            act.Should().Throw<Exception>();
        }

        [Fact]
        public void MapFromDTO_WhenIdIsSet_ThrowsException()
        {
            const string someName = "Sailors Suprise";
            var someGuid = Guid.NewGuid();
            DishDTO dto = new(someName) { Id = someGuid };

            Action act = () => _mapper.MapFromDTO(dto);

            act.Should().Throw<Exception>();
        }
    }
}
