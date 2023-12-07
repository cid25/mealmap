using Mealmap.Api.Common;
using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.DishAggregate;

namespace Mealmap.Api.Dishes;

public class DishQueryResponder(IRepository<Dish> repository, ILogger<DishesController> logger, IOutputMapper<DishDTO, Dish> outputMapper)
    : IQueryResponder<DishQuery, DishDTO?>
{
    public async Task<DishDTO?> RespondTo(DishQuery query)
    {
        var dish = repository.GetSingleById(query.Id);

        if (dish == null)
        {
            logger.LogInformation("Attempt to retrieve non-existing dish");
            return null;
        }
        else return outputMapper.FromEntity(dish);
    }
}
