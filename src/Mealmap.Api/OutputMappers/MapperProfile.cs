using AutoMapper;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Domain.DishAggregate;
using Mealmap.Domain.MealAggregate;

namespace Mealmap.Api.OutputMappers;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<string?, byte[]?>().ConvertUsing(s => s == null ? null : Convert.FromBase64String(s));
        CreateMap<byte[]?, string?>().ConvertUsing(b => b == null ? string.Empty : Convert.ToBase64String(b));

        CreateMap<UnitOfMeasurement, string>()
            .ConvertUsing(unit => unit.Stringify());
        CreateMap<string, UnitOfMeasurement>()
            .ConvertUsing(str => new UnitOfMeasurement(str));
        CreateMap<Ingredient, IngredientDTO>();
        CreateMap<IngredientDTO, Ingredient>();
        CreateMap<Dish, Guid>();
        CreateMap<DishDTO, Dish>()
            .ForMember(dish => dish.Image, opt => opt.Ignore())
            .ForMember(dish => dish.Version, opt => { opt.MapFrom(dto => dto.ETag); });
        CreateMap<Dish, DishDTO>()
            .ForMember(dto => dto.ImageUrl, opt => opt.Ignore())
            .ForMember(dto => dto.ETag, opt => { opt.MapFrom(dish => dish.Version); });

        CreateMap<Course, CourseDTO>().ReverseMap();
        CreateMap<Meal, MealDTO>();
    }
}
