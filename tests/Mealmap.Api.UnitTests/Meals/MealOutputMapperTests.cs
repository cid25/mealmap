using AutoMapper;
using Mealmap.Api.Meals;
using Mealmap.Api.Shared;

namespace Mealmap.Api.UnitTests.Meals;

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
