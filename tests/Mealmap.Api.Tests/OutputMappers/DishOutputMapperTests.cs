using AutoMapper;
using FluentAssertions;
using Mealmap.Api.OutputMappers;
using Mealmap.Domain.DishAggregate;
using Moq;

namespace Mealmap.Api.UnitTests.OutputMappers;

public class DishOutputMapperTests
{
    private readonly IMapper _baseMapper;

    public DishOutputMapperTests()
    {
        _baseMapper = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>()).CreateMapper();
    }

    [Fact]
    public void MapFromEntity_ReturnsProperDto()
    {
        var mapper = new DishOutputMapper(
            _baseMapper,
            Mock.Of<IRequestContext>(m => m.Scheme == "https" && m.Host == "test.com" && m.Port == 443)
        );

        const string SomeName = "Sailors Suprise";
        var someGuid = Guid.NewGuid();
        var dish = new Dish(SomeName)
        {
            Id = someGuid,
            Image = new DishImage(content: new byte[1], contentType: "image/jpeg"),
            Version = new byte[] { 0x01 }
        };

        var dto = mapper.FromEntity(dish);

        dto.Id.Should().Be(someGuid);
        dto.Name.Should().Be(SomeName);
        dto.ImageUrl!.ToString().Should().EndWith(someGuid.ToString() + "/image");
        dto.ETag.Should().NotBeNullOrEmpty();
    }
}
