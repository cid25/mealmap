using AutoMapper;
using Mealmap.Api.Shared;
using Mealmap.Domain.DishAggregate;

namespace Mealmap.Api.Dishes;

public class DishOutputMapper : IOutputMapper<DishDTO, Dish>
{
    private readonly IMapper _mapper;
    private readonly IRequestContext _context;

    public DishOutputMapper(IMapper mapper, IRequestContext requestContext)
    {
        _mapper = mapper;
        _context = requestContext;
    }

    public DishDTO FromEntity(Dish entity)
    {
        var dto = _mapper.Map<DishDTO>(entity);

        if (entity.Image != null)
        {
            var builder = new UriBuilder()
            {
                Scheme = _context.Scheme,
                Host = _context.Host,
                Port = _context.Port,
                Path = "/api/dishes/" + dto.Id + "/image"
            };

            dto.ImageUrl = builder.Uri;
        }

        return dto;
    }

    public IEnumerable<DishDTO> FromEntities(IEnumerable<Dish> entities)
    {
        List<DishDTO> dtos = new();

        dtos.AddRange(entities.Select(FromEntity));

        return dtos;
    }
}
