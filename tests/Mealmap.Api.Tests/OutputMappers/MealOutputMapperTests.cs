using AutoMapper;
using FluentAssertions;
using Mealmap.Api.OutputMappers;

namespace Mealmap.Api.UnitTests.OutputMappers;

public class MealOutputMapperTests
{
    private readonly MealOutputMapper _mealMapper;

    public MealOutputMapperTests()
    {
        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<AutomapperProfile>()).CreateMapper();

        _mealMapper = new MealOutputMapper(mapper);
    }

    [Fact]
    public void Dummy()
    {
        _mealMapper.Should().BeOfType<MealOutputMapper>();
    }
}
