using AutoMapper;
using FluentAssertions;
using Mealmap.Api.Controllers;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Model;
using Microsoft.AspNetCore.Mvc;

namespace Mealmap.Api.UnitTests
{
    public class DishesControllerTests
    {
        FakeDishRepository _repository;
        DishesController _controller;
        
        public DishesControllerTests()
        {
            _repository = new FakeDishRepository();
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<MealmapMapperProfile>());
            _controller = new DishesController(_repository, mapperConfig.CreateMapper());

            const string firstGuid = "00000000-0000-0000-0000-000000000001";
            _repository.Add(new Guid(firstGuid), new Dish() {Id = new Guid(firstGuid)} );
        }

        [Fact]
        public void GetDishes_ReturnsDishDTOs()
        {
            var result = _controller.GetDishes();

            result.Should().BeOfType<ActionResult<IEnumerable<DishDTO>>>();
            result.Value.Should().HaveCountGreaterThan(0);
        }
    }
}
