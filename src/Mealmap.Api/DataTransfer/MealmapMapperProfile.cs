using AutoMapper;
using Mealmap.Model;


namespace Mealmap.Api.DataTransfer
{
    public class MealmapMapperProfile : Profile
    {
        public MealmapMapperProfile()
        {
            CreateMap<Dish, Guid>();
            CreateMap<MealDTO, Meal>().ForMember(d => d.Dish, opt => opt.Ignore()).ForSourceMember(s => s.Dish, opt => opt.DoNotValidate());
            CreateMap<Meal, MealDTO>();
            CreateMap<Dish, DishDTO>().ReverseMap();
        }
    }
}
