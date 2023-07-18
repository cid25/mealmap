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
                .ForMember(meal => meal.Dish, opt => opt.Ignore())
                .ForSourceMember(dto => dto.DishId, opt => opt.DoNotValidate());
            CreateMap<Meal, MealDTO>()
                .ForMember(dto => dto.DishId, opt => opt.MapFrom(meal => meal.Dish!.Id));
            CreateMap<DishDTO, Dish>()
                .ForMember(dish => dish.Image, opt => opt.Ignore());
            CreateMap<Dish, DishDTO>()
                .ForMember(dto => dto.ImageUrl, opt => opt.Ignore());
            CreateMap<UnitOfMeasurement, string>()
                .ConvertUsing(unit => unit.Stringify());
            CreateMap<string, UnitOfMeasurement>()
                .ConvertUsing(str => new UnitOfMeasurement(str));
            CreateMap<Ingredient, IngredientDTO>();
            CreateMap<IngredientDTO, Ingredient>();
        }
    }
}
