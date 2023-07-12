using AutoMapper;
using Mealmap.Model;
using Mealmap.Api.DataTransfer;
using Mealmap.Api.Formatters;
using FluentAssertions;

namespace Mealmap.Api.UnitTests
{
    public class DishMapperTests
    {
        private readonly DishMapper _mapper;

        public DishMapperTests()
        {
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>()).CreateMapper();
            _mapper = new DishMapper(mapper);
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

            var dto = _mapper.MapFromEntity(dish);

            dto.Id.Should().Be(guid);
            dto.Name.Should().Be(name);
        }
    }
}
