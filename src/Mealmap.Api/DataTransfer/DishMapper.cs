using AutoMapper;
using Mealmap.Model;
using Microsoft.AspNetCore.Mvc;

namespace Mealmap.Api.DataTransfer
{
    public class DishMapper
    {
        private readonly IMapper _mapper;

        public DishMapper(IMapper mapper)
        {
            _mapper = mapper;
        }

        public Dish MapFromDTO(DishDTO dto)
        {
            var dish = _mapper.Map<Dish>(dto);

            return dish;
        }

        public DishDTO MapFromEntity(Dish dish)
        {
            var dto = _mapper.Map<DishDTO>(dish);

            return dto;
        }

        public List<DishDTO> MapFromEntities(IEnumerable<Dish> meals)
        {
            return _mapper.Map<IEnumerable<Dish>, List<DishDTO>>(meals);
        }
    }
}
