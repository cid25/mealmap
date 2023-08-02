using AutoMapper;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Domain.Common;
using Mealmap.Domain.DishAggregate;
using Mealmap.Domain.MealAggregate;

namespace Mealmap.Api.OutputMappers;

public class AutomapperProfile : Profile
{
    public AutomapperProfile()
    {
        CreateMap<EntityVersion, string?>().ConvertUsing(ev => ev.AsString());

        CreateMap<UnitOfMeasurement, string>()
            .ConvertUsing(unit => unit.Stringify());
        CreateMap<Ingredient, IngredientDTO>();
        CreateMap<Dish, Guid>();
        CreateMap<Dish, DishDTO>()
            .ForMember(dto => dto.ImageUrl, opt => opt.Ignore())
            .ForMember(dto => dto.ETag, opt => { opt.MapFrom(dish => dish.Version); });

        CreateMap<Course, CourseDTO>();
        CreateMap<Meal, MealDTO>()
            .ForMember(dto => dto.ETag, opt => { opt.MapFrom(meal => meal.Version); });
    }
}
