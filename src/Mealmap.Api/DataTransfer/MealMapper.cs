using AutoMapper;
using Mealmap.Model;

namespace Mealmap.Api.DataTransfer
{
    public class MealMapper
    {
        private readonly IMapper _mapper;
        private readonly IDishRepository _dishRepository;

        public MealMapper(IMapper mapper, IDishRepository dishRepository)
        {
            _mapper = mapper;
            _dishRepository = dishRepository;
        }

        public Meal MapFromDTO(MealDTO dto)
        {
            var meal = _mapper.Map<Meal>(dto);

            if (dto.Dish != null && dto.Dish != Guid.Empty)
                meal.Dish = _dishRepository.GetById((Guid)dto.Dish);

            return meal;
        }

        public MealDTO MapFromEntity(Meal meal)
        {
            return _mapper.Map<MealDTO>(meal);
        }

        public List<MealDTO> MapFromEntities(IEnumerable<Meal> meals)
        {
            return _mapper.Map<IEnumerable<Meal>, List<MealDTO>>(meals);
        }
    }
}
