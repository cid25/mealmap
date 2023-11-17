using Mealmap.Api.Shared;
using Mealmap.Domain.MealAggregate;

namespace Mealmap.Api.Meals;

public class MealQueryResponder(IMealRepository repository, ILogger<MealsController> logger, IOutputMapper<MealDTO, Meal> outputMapper)
    : IQueryResponder<MealQuery, MealDTO?>
{
    public async Task<MealDTO?> RespondTo(MealQuery query)
    {
        var meal = repository.GetSingleById(query.Id);

        if (meal == null)
        {
            logger.LogInformation($"Client attempt to retrieve non-existing meal with Id {query.Id}.");
            return null;
        }

        return outputMapper.FromEntity(meal);
    }
}
