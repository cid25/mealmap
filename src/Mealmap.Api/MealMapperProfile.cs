using AutoMapper;
using Mealmap.Model;
using Mealmap.Api.DataTransferObjects;


namespace Mealmap.Api
{
    public class MealMapperProfile : Profile
    {
        public MealMapperProfile()
        {
            CreateMap<MealDTO, Meal>();
            CreateMap<Meal, MealDTO>();
        }
    }
}
