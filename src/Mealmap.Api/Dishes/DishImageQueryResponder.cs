using Mealmap.Api.Common;
using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.DishAggregate;

namespace Mealmap.Api.Dishes;

public class DishImageQueryResponder(IRepository<Dish> repository, IOutputMapper<DishDTO, Dish> outputMapper)
    : IQueryResponder<DishImageQuery, (DishDTO?, Image?)>
{
    public async Task<(DishDTO?, Image?)> RespondTo(DishImageQuery query)
    {
        var dish = repository.GetSingleById(query.Id);

        if (dish == null) return (null, null);

        var dto = outputMapper.FromEntity(dish);
        if (dish.Image == null) return (dto, null);

        var image = new Image(dish.Image.Content, dish.Image.ContentType);
        return (dto, image);
    }
}
