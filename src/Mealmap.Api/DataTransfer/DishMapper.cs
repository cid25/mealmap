using AutoMapper;
using Mealmap.Model;

namespace Mealmap.Api.DataTransfer
{
    public class DishMapper : IDishMapper
    {
        private readonly ILogger<DishMapper> _logger;
        private readonly IMapper _mapper;
        private readonly IRequestContext _context;

        public DishMapper(ILogger<DishMapper> logger, IMapper mapper, IRequestContext requestContext)
        {
            _logger = logger;
            _mapper = mapper;
            _context = requestContext;
        }

        public Dish MapFromDTO(DishDTO dto)
        {
            if (String.IsNullOrWhiteSpace(dto.Name))
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

        public DishDTO MapFromEntity(Dish dish)
        {
            var dto = _mapper.Map<DishDTO>(dish);

            if (dish.Image != null)
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

        public List<DishDTO> MapFromEntities(IEnumerable<Dish> dishes)
        {
            List<DishDTO> dtos = new();

            foreach (var dish in dishes)
                dtos.Add(MapFromEntity(dish));

            return dtos;
        }
    }
}
