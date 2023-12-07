using AutoMapper;
using Mealmap.Api.Common;
using Mealmap.Domain.DishAggregate;

namespace Mealmap.Api.Dishes;

public class DishOutputMapper(IMapper mapper, IRequestContext requestContext)
    : IOutputMapper<DishDTO, Dish>
{
    public DishDTO FromEntity(Dish entity)
    {
        var dto = mapper.Map<DishDTO>(entity);

        if (entity.Image != null)
        {
            var builder = new UriBuilder()
            {
                Scheme = requestContext.Scheme,
                Host = requestContext.Host,
                Port = requestContext.Port,
                Path = "/api/dishes/" + dto.Id + "/image"
            };

            dto.ImageUrl = builder.Uri;
        }

        return dto;
    }

    public IEnumerable<DishDTO> FromEntities(IEnumerable<Dish> entities)
    {
        List<DishDTO> dtos = [];

        dtos.AddRange(entities.Select(FromEntity));

        return dtos;
    }
}
