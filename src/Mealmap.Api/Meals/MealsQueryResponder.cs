using Mealmap.Api.Shared;
using Mealmap.Domain.MealAggregate;

namespace Mealmap.Api.Meals;

public class MealsQueryResponder(IMealRepository repository, IOutputMapper<MealDTO, Meal> outputMapper)
    : IQueryResponder<MealsQuery, IEnumerable<MealDTO>>
{
    public async Task<IEnumerable<MealDTO>> RespondTo(MealsQuery query)
    {
        var meals = repository.GetAll(query.FromDate, query.ToDate);

        if (!meals.Any())
            return Enumerable.Empty<MealDTO>();
        return outputMapper.FromEntities(meals);
    }
}
