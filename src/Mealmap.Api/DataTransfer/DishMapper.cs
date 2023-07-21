using AutoMapper;
using Mealmap.Model;

namespace Mealmap.Api.DataTransfer
{
    public class DishMapper
    {
        private readonly ILogger<DishMapper> _logger;
        private readonly IMapper _mapper;
        private readonly HttpContext? _httpContext;

        public DishMapper(ILogger<DishMapper> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _mapper = mapper;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public Dish MapFromDTO(DishDTO dto)
        {
            if (dto.Id != Guid.Empty && dto.Id != null)
            {
                _logger.LogInformation("Client attempt to create dish with pre-existing Id {Id}", dto.Id);
                throw new InvalidOperationException("Id is not allowed as part of a request.");
            }

            if (String.IsNullOrWhiteSpace(dto.Name))
            {
                _logger.LogInformation("Attempt to create dish with empty name");
                throw new InvalidOperationException("Name is a mandatory field.");
            }

            dto = dto with { Id = Guid.NewGuid() };
            var dish = _mapper.Map<Dish>(dto);

            return dish;
        }

        public DishDTO MapFromEntity(Dish dish)
        {
            if (_httpContext is null)
                throw new InvalidOperationException($"Mapping from DishDTO to Dish cannot be executed without an httpContext.");

            var dto = _mapper.Map<DishDTO>(dish);

            if (dish.Image != null)
            {
                var builder = new UriBuilder()
                {
                    Scheme = _httpContext.Request.Scheme,
                    Host = _httpContext.Request.Host.Host,
                    Port = _httpContext.Request.Host.Port ?? -1,
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
