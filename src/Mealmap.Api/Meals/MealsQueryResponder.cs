using Mealmap.Api.Shared;
using Mealmap.Domain.MealAggregate;

namespace Mealmap.Api.Meals;

public class MealsQueryResponder : IQueryResponder<MealsQuery, IEnumerable<MealDTO>>
{
    private readonly IMealRepository _repository;
    private readonly IOutputMapper<MealDTO, Meal> _outputMapper;

    public MealsQueryResponder(IMealRepository repository, IOutputMapper<MealDTO, Meal> outputMapper)
    {
        _repository = repository;
        _outputMapper = outputMapper;
    }

    public async Task<IEnumerable<MealDTO>> RespondTo(MealsQuery query)
    {
        var meals = _repository.GetAll(query.FromDate, query.ToDate);

        if (!meals.Any()) return Enumerable.Empty<MealDTO>();
        return _outputMapper.FromEntities(meals);
    }
}
