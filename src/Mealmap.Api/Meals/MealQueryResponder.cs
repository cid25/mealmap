using Mealmap.Api.Shared;
using Mealmap.Domain.MealAggregate;

namespace Mealmap.Api.Meals;

public class MealQueryResponder : IQueryResponder<MealQuery, MealDTO?>
{
    private readonly IMealRepository _repository;
    private readonly ILogger<MealsController> _logger;
    private readonly IOutputMapper<MealDTO, Meal> _outputMapper;

    public MealQueryResponder(IMealRepository repository, ILogger<MealsController> logger, IOutputMapper<MealDTO, Meal> outputMapper)
    {
        _repository = repository;
        _logger = logger;
        _outputMapper = outputMapper;
    }

    public async Task<MealDTO?> RespondTo(MealQuery query)
    {
        var meal = _repository.GetSingleById(query.Id);

        if (meal == null)
        {
            _logger.LogInformation("Client attempt to retrieve non-existing meal with Id {Id]", query.Id);
            return null;
        }

        return _outputMapper.FromEntity(meal);
    }
}
