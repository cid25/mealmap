using AutoMapper;
using Mealmap.Model;


namespace Mealmap.Api.DataTransfer
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Dish, Guid>();
            CreateMap<MealDTO, Meal>()
                .ForMember(d => d.Dish, opt => opt.Ignore())
                .ForSourceMember(s => s.Dish, opt => opt.DoNotValidate());
            CreateMap<Meal, MealDTO>()
                .ForMember(d => d.Dish, opt => opt.MapFrom(src => src.Dish!.Id));
            CreateMap<Dish, DishDTO>()
                .ReverseMap();
        }
    }
}
