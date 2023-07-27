using AutoMapper;
using FluentAssertions;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Api.InputMappers;
using Mealmap.Api.OutputMappers;
using Microsoft.Extensions.Logging;
using Moq;

namespace Mealmap.Api.UnitTests;

public class DishInputMapperTests
{
    private readonly ILogger<DishInputMapper> _logger;
    private readonly IMapper _baseMapper;

    public DishInputMapperTests()
    {
        _logger = Mock.Of<ILogger<DishInputMapper>>();
        _baseMapper = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>()).CreateMapper();
    }

    [Fact]
    public void FromDataTransferObject_WhenMethodPostAndIdSet_ThrowsException()
    {
        var mapper = new DishInputMapper(_logger, _baseMapper, Mock.Of<IRequestContext>(m => m.Method == "POST"));
        const string someName = "Sailors Suprise";
        var someGuid = Guid.NewGuid();
        DishDTO dto = new(someName) { Id = someGuid };

        Action act = () => mapper.FromDataTransferObject(dto);

        act.Should().Throw<Exception>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void FromDataTransferObject_WhenNameIsntSet_ThrowsException(string name)
    {
        var mapper = new DishInputMapper(_logger, _baseMapper, Mock.Of<IRequestContext>());
        var someGuid = Guid.NewGuid();
        DishDTO dto = new(name);

        Action act = () => mapper.FromDataTransferObject(dto);

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void FromDataTransferObject_WhenMethodPut_IncludesVersionFromEtag()
    {
        var etag = "AAAAAAAAB9E=";
        var context = Mock.Of<IRequestContext>(m => m.Method == "PUT" && m.IfMatchHeader == etag);
        var mapper = new DishInputMapper(_logger, _baseMapper, context);

        const string someName = "Sailors Suprise";
        var someGuid = Guid.NewGuid();
        DishDTO dto = new(someName) { Id = someGuid };

        var result = mapper.FromDataTransferObject(dto);

        result.Version.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void FromDataTransferObject_WhenMethodPut_RetainsId()
    {
        var etag = "AAAAAAAAB9E=";
        var context = Mock.Of<IRequestContext>(m => m.Method == "PUT" && m.IfMatchHeader == etag);
        var mapper = new DishInputMapper(_logger, _baseMapper, context);

        const string someName = "Sailors Suprise";
        var someGuid = Guid.NewGuid();
        DishDTO dto = new(someName) { Id = someGuid };

        var result = mapper.FromDataTransferObject(dto);

        result.Id.Should().Be(someGuid);
    }

    [Fact]
    public void FromDataTransferObject_WhenMethodPost_GeneratesId()
    {
        var context = Mock.Of<IRequestContext>(m => m.Method == "POST");
        var mapper = new DishInputMapper(_logger, _baseMapper, context);

        const string someName = "Sailors Suprise";
        DishDTO dto = new(someName);

        var result = mapper.FromDataTransferObject(dto);

        result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void FromDataTransferObject_WhenMethodPost_EtagNulled()
    {
        var etag = "AAAAAAAAB9E=";
        var context = Mock.Of<IRequestContext>(m => m.Method == "POST" && m.IfMatchHeader == etag);
        var mapper = new DishInputMapper(_logger, _baseMapper, context);

        const string someName = "Sailors Suprise";
        var someGuid = Guid.NewGuid();
        DishDTO dto = new(someName) { ETag = etag };

        var result = mapper.FromDataTransferObject(dto);

        result.Version.Should().BeNull();
    }
}
