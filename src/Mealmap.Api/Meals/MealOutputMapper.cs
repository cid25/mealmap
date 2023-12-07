using AutoMapper;
using Mealmap.Api.Common;
using Mealmap.Domain.MealAggregate;

namespace Mealmap.Api.Meals;

public class MealOutputMapper(IMapper mapper) : IOutputMapper<MealDTO, Meal>
{
    public MealDTO FromEntity(Meal entity)
    {
        return mapper.Map<MealDTO>(entity);
    }

    public IEnumerable<MealDTO> FromEntities(IEnumerable<Meal> entities)
    {
        return mapper.Map<IEnumerable<Meal>, List<MealDTO>>(entities);
    }
}
