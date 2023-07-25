using AutoMapper;
using FluentAssertions;
using Mealmap.Api.DataTransfer;
using Mealmap.Model;
using Microsoft.Extensions.Logging;
using Moq;

namespace Mealmap.Api.UnitTests
{
    public class DishMapperTests
    {
        private readonly ILogger<DishMapper> _logger;
        private readonly IMapper _baseMapper;

        public DishMapperTests()
        {
            _logger = Mock.Of<ILogger<DishMapper>>();
            _baseMapper = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>()).CreateMapper();
        }

        [Fact]
        public void MapFromEntity_ReturnsProperDto()
        {
            var mapper = new DishMapper(
                _logger,
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

            var dto = mapper.MapFromEntity(dish);

            dto.Id.Should().Be(someGuid);
            dto.Name.Should().Be(SomeName);
            dto.ImageUrl!.ToString().Should().EndWith(someGuid.ToString() + "/image");
            dto.ETag.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void MapFromDTO_WhenMethodPostAndIdSet_ThrowsException()
        {
            var mapper = new DishMapper(_logger, _baseMapper, Mock.Of<IRequestContext>(m => m.Method == "POST"));
            const string someName = "Sailors Suprise";
            var someGuid = Guid.NewGuid();
            DishDTO dto = new(someName) { Id = someGuid };

            Action act = () => mapper.MapFromDTO(dto);

            act.Should().Throw<Exception>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void MapFromDTO_WhenNameIsntSet_ThrowsException(string name)
        {
            var mapper = new DishMapper(_logger, _baseMapper, Mock.Of<IRequestContext>());
            var someGuid = Guid.NewGuid();
            DishDTO dto = new(name);

            Action act = () => mapper.MapFromDTO(dto);

            act.Should().Throw<Exception>();
        }

        [Fact]
        public void MapFromDTO_WhenMethodPut_IncludesVersionFromEtag()
        {
            var etag = "AAAAAAAAB9E=";
            var context = Mock.Of<IRequestContext>(m => m.Method == "PUT" && m.IfMatchHeader == etag);
            var mapper = new DishMapper(_logger, _baseMapper, context);

            const string someName = "Sailors Suprise";
            var someGuid = Guid.NewGuid();
            DishDTO dto = new(someName) { Id = someGuid };

            var result = mapper.MapFromDTO(dto);

            result.Version.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void MapFromDTO_WhenMethodPut_RetainsId()
        {
            var etag = "AAAAAAAAB9E=";
            var context = Mock.Of<IRequestContext>(m => m.Method == "PUT" && m.IfMatchHeader == etag);
            var mapper = new DishMapper(_logger, _baseMapper, context);

            const string someName = "Sailors Suprise";
            var someGuid = Guid.NewGuid();
            DishDTO dto = new(someName) { Id = someGuid };

            var result = mapper.MapFromDTO(dto);

            result.Id.Should().Be(someGuid);
        }

        [Fact]
        public void MapFromDTO_WhenMethodPost_GeneratesId()
        {
            var context = Mock.Of<IRequestContext>(m => m.Method == "POST");
            var mapper = new DishMapper(_logger, _baseMapper, context);

            const string someName = "Sailors Suprise";
            DishDTO dto = new(someName);

            var result = mapper.MapFromDTO(dto);

            result.Id.Should().NotBeEmpty();
        }

        [Fact]
        public void MapFromDTO_WhenMethodPost_EtagNulled()
        {
            var etag = "AAAAAAAAB9E=";
            var context = Mock.Of<IRequestContext>(m => m.Method == "POST" && m.IfMatchHeader == etag);
            var mapper = new DishMapper(_logger, _baseMapper, context);

            const string someName = "Sailors Suprise";
            var someGuid = Guid.NewGuid();
            DishDTO dto = new(someName) { ETag = etag };

            var result = mapper.MapFromDTO(dto);

            result.Version.Should().BeNull();
        }
    }
}
