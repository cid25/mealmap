using AutoMapper;
using Moq;
using Mealmap.Model;
using Mealmap.Api.DataTransfer;
using FluentAssertions;

namespace Mealmap.Api.UnitTests
{
    public class MealMapperTests
    {
        private readonly MealMapper _mealMapper;

        public MealMapperTests()
        {
            Guid dishGuid = new ("00000000-0000-0000-0000-000000000001");
            var _dishRepositoryMock = Mock.Of<IDishRepository>(m =>
                m.GetById(dishGuid) == new Dish("Krabby Patty") { Id = dishGuid });
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>()).CreateMapper();

            _mealMapper = new MealMapper(mapper, _dishRepositoryMock);
        }

        [Fact]
        public void MapFromDTO_ReturnsMealWithDish()
        {
            var mealGuid = Guid.NewGuid();
            var mealDate = DateOnly.FromDateTime(DateTime.Now);
            var dto = new MealDTO()
            {
                Date = mealDate,
                Dish = new Guid("00000000-0000-0000-0000-000000000001"),
                Id = mealGuid
            };

            var meal = _mealMapper.MapFromDTO(dto);

            meal.Id.Should().Be(mealGuid);
            meal.Date.Should().Be(mealDate);
            meal.Dish.Should().NotBeNull();
        }
    }
}
