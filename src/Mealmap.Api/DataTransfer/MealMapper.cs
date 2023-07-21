using AutoMapper;
using Mealmap.Model;

namespace Mealmap.Api.DataTransfer
{
    public class MealMapper
    {
        private readonly ILogger<MealMapper> _logger;
        private readonly IMapper _mapper;
        private readonly IDishRepository _dishRepository;

        public MealMapper(ILogger<MealMapper> logger, IMapper mapper, IDishRepository dishRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _dishRepository = dishRepository;
        }

        public Meal MapFromDTO(MealDTO dto)
        {
            if (dto.Id != Guid.Empty && dto.Id != null)
            {
                _logger.LogInformation("Client attempt to create meal with pre-existing Id {Id}", dto.Id);
                throw new InvalidOperationException("Id is not allowed as part of a request.");
            }

            dto = dto with { Id = Guid.NewGuid() };

            var meal = _mapper.Map<Meal>(dto);

            if (dto.DishId != null && dto.DishId != Guid.Empty)
            {
                var dish = _dishRepository.GetById((Guid)dto.DishId);

                if (dish == null)
                {
                    _logger.LogInformation("Client attempt to create meal with non-existing Dish of ID {Id}", dto.DishId);
                    throw new InvalidOperationException("Dish doesn't exist.");
                }

                meal.Dish = dish;
            }

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
