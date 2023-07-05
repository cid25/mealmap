using AutoMapper;
using Mealmap.Model;
using Mealmap.Api.DataTransferObjects;


namespace Mealmap.Api
{
    public class MealmapMapperProfile : Profile
    {
        public MealmapMapperProfile()
        {
            CreateMap<MealDTO, Meal>();
            CreateMap<Meal, MealDTO>();
            CreateMap<DishDTO, Dish>();
            CreateMap<Dish, DishDTO>();
        }
    }
}
