using AutoMapper;
using FluentAssertions;
using Mealmap.Api.DataTransfer;
using Mealmap.Model;
using Microsoft.Extensions.Logging;
using Moq;

namespace Mealmap.Api.UnitTests
{
    public class MealMapperTests
    {
        private readonly MealMapper _mealMapper;

        public MealMapperTests()
        {
            Guid dishGuid = new("00000000-0000-0000-0000-000000000001");
            var _dishRepositoryMock = Mock.Of<IDishRepository>(m =>
                m.GetSingle(dishGuid) == new Dish("Krabby Patty") { Id = dishGuid });
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>()).CreateMapper();

            _mealMapper = new MealMapper(Mock.Of<ILogger<MealMapper>>(), mapper, _dishRepositoryMock);
        }

        [Fact]
        public void MapFromDTO_ReturnsMealWithDish()
        {
            var mealDate = DateOnly.FromDateTime(DateTime.Now);
            var dto = new MealDTO()
            {
                DiningDate = mealDate,
                DishId = new Guid("00000000-0000-0000-0000-000000000001"),
            };

            var meal = _mealMapper.MapFromDTO(dto);

            meal.DiningDate.Should().Be(mealDate);
            meal.Dish.Should().NotBeNull();
        }

        [Fact]
        public void MapFromDTO_WhenIdIsSet_ThrowsException()
        {
            var someGuid = Guid.NewGuid();
            var someMealDate = DateOnly.FromDateTime(DateTime.Now);
            var dto = new MealDTO()
            {
                Id = someGuid,
                DiningDate = someMealDate,
            };

            Action act = () => _mealMapper.MapFromDTO(dto);

            act.Should().Throw<Exception>();
        }

        [Fact]
        public void MapFromDTO_WhenDishDoesntExist_ThrowsException()
        {
            var nonExistingDishGuid = Guid.NewGuid();
            var mealDate = DateOnly.FromDateTime(DateTime.Now);
            var dto = new MealDTO()
            {
                DiningDate = mealDate,
                DishId = nonExistingDishGuid,
            };

            Action act = () => _mealMapper.MapFromDTO(dto);

            act.Should().Throw<Exception>();
        }
    }
}
