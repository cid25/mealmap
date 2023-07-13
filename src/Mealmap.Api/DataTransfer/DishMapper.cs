using AutoMapper;
using Mealmap.Api.Controllers;
using Mealmap.Model;
using Microsoft.AspNetCore.Mvc;

namespace Mealmap.Api.DataTransfer
{
    public class DishMapper
    {
        private readonly IMapper _mapper;
        private readonly HttpContext? _httpContext;

        public DishMapper(IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public Dish MapFromDTO(DishDTO dto)
        {
            var dish = _mapper.Map<Dish>(dto);

            return dish;
        }

        public DishDTO MapFromEntity(Dish dish)
        {
            if (_httpContext is null)
                throw new ArgumentNullException();

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
            List<DishDTO> dtos = new ();

            foreach (var dish in dishes)
                dtos.Add(MapFromEntity(dish));

            return dtos;
        }
    }
}
