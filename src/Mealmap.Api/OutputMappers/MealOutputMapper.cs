using AutoMapper;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Domain.MealAggregate;

namespace Mealmap.Api.OutputMappers;

public class MealOutputMapper : IOutputMapper<MealDTO, Meal>
{
    private readonly IMapper _mapper;

    public MealOutputMapper(IMapper mapper)
    {
        _mapper = mapper;
    }

    public MealDTO FromEntity(Meal entity)
    {
        return _mapper.Map<MealDTO>(entity);
    }

    public IEnumerable<MealDTO> FromEntities(IEnumerable<Meal> entities)
    {
        return _mapper.Map<IEnumerable<Meal>, List<MealDTO>>(entities);
    }
}
