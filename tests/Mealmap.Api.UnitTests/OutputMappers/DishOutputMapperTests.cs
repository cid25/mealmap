﻿using AutoMapper;
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
        _baseMapper = new MapperConfiguration(cfg => cfg.AddProfile<AutomapperProfile>()).CreateMapper();
    }

    [Fact]
    public void MapFromEntity_ReturnsProperDto()
    {
        var mapper = new DishOutputMapper(
            _baseMapper,
            Mock.Of<IRequestContext>(m => m.Scheme == "https" && m.Host == "test.com" && m.Port == 443)
        );

        const string SomeName = "Sailors Suprise";
        var aGuid = Guid.NewGuid();
        DishFactory factory = new();
        var dish = factory.CreateDishWith(id: aGuid, name: SomeName, description: null, servings: 2);
        dish.SetImage(new byte[1], "image/jpeg");
        dish.Version.Set(new byte[] { 0x01 });

        var dto = mapper.FromEntity(dish);

        dto.Id.Should().Be(aGuid);
        dto.Name.Should().Be(SomeName);
        dto.ImageUrl!.ToString().Should().EndWith(aGuid.ToString() + "/image");
        dto.ETag.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void FromEntities_ReturnsCorrectCount()
    {
        // Arrange
        var mapper = new DishOutputMapper(
            _baseMapper,
            Mock.Of<IRequestContext>(m => m.Scheme == "https" && m.Host == "test.com" && m.Port == 443)
        );
        List<Dish> dtos = new();
        DishFactory factory = new();

        for (int i = 0; i < 10; i++)
            dtos.Add(factory.CreateDishWith("Dish" + i, null, i));

        // Act
        var result = mapper.FromEntities(dtos);

        // Assert
        result.Should().HaveCount(10);
    }
}