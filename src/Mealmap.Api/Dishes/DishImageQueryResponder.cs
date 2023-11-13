using Mealmap.Api.Shared;
using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.DishAggregate;

namespace Mealmap.Api.Dishes;

public class DishImageQueryResponder : IQueryResponder<DishImageQuery, (DishDTO?, Image?)>
{
    private readonly IRepository<Dish> _repository;
    private readonly IOutputMapper<DishDTO, Dish> _outputMapper;

    public DishImageQueryResponder(IRepository<Dish> repository, IOutputMapper<DishDTO, Dish> outputMapper)
    {
        _repository = repository;
        _outputMapper = outputMapper;
    }

    public async Task<(DishDTO?, Image?)> RespondTo(DishImageQuery query)
    {
        var dish = _repository.GetSingleById(query.Id);

        if (dish == null) return (null, null);

        var dto = _outputMapper.FromEntity(dish);
        if (dish.Image == null) return (dto, null);

        var image = new Image(dish.Image.Content, dish.Image.ContentType);
        return (dto, image);
    }
}
