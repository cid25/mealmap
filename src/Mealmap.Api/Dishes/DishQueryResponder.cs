using Mealmap.Api.Shared;
using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.DishAggregate;

namespace Mealmap.Api.Dishes;

public class DishQueryResponder : IQueryResponder<DishQuery, DishDTO?>
{
    private readonly ILogger<DishesController> _logger;
    private readonly IRepository<Dish> _repository;
    private readonly IOutputMapper<DishDTO, Dish> _outputMapper;

    public DishQueryResponder(IRepository<Dish> repository, ILogger<DishesController> logger, IOutputMapper<DishDTO, Dish> outputMapper)
    {
        _repository = repository;
        _logger = logger;
        _outputMapper = outputMapper;
    }

    public async Task<DishDTO?> RespondTo(DishQuery query)
    {
        var dish = _repository.GetSingleById(query.Id);

        if (dish == null)
        {
            _logger.LogInformation("Attempt to retrieve non-existing dish");
            return null;
        }
        else return _outputMapper.FromEntity(dish);
    }
}
