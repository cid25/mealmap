using AutoMapper;
using Mealmap.Api.Dishes;
using Mealmap.Api.Meals;
using Mealmap.Domain.Common;
using Mealmap.Domain.DishAggregate;
using Mealmap.Domain.MealAggregate;

namespace Mealmap.Api.Common;

public class AutomapperProfile : Profile
{
    public AutomapperProfile()
    {
        CreateMap<EntityVersion, string?>().ConvertUsing(ev => ev.AsString());

        CreateMap<Ingredient, IngredientDTO>();
        CreateMap<Dish, DishDTO>()
            .ForMember(dto => dto.ImageUrl, opt => opt.Ignore())
            .ForMember(dto => dto.ETag, opt => { opt.MapFrom(dish => dish.Version); });

        CreateMap<Course, CourseDTO>();
        CreateMap<Meal, MealDTO>()
            .ForMember(dto => dto.ETag, opt => { opt.MapFrom(meal => meal.Version); });
    }
}
