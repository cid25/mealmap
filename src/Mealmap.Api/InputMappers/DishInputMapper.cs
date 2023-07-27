using AutoMapper;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Domain.DishAggregate;

namespace Mealmap.Api.InputMappers;

public class DishInputMapper : IInputMapper<Dish, DishDTO>
{
    private readonly ILogger<DishInputMapper> _logger;
    private readonly IMapper _mapper;
    private readonly IRequestContext _context;

    public DishInputMapper(ILogger<DishInputMapper> logger, IMapper mapper, IRequestContext requestContext)
    {
        _logger = logger;
        _mapper = mapper;
        _context = requestContext;
    }

    public Dish FromDataTransferObject(DishDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            _logger.LogInformation("Client attempt to create or update dish with empty name");
            throw new InvalidOperationException("Field name is mandatory.");
        }

        if (_context.Method == "POST" && dto.Id != Guid.Empty && dto.Id != null)
        {
            _logger.LogInformation("Client attempt to create dish with pre-existing Id {Id}", dto.Id);
            throw new InvalidOperationException("Field id is not allowed.");
        }

        dto = dto with
        {
            Id = _context.Method == "POST" ? Guid.NewGuid() : dto.Id,
            ETag = _context.Method == "PUT" ? _context.IfMatchHeader : null
        };

        var dish = _mapper.Map<Dish>(dto);

        return dish;
    }
}
