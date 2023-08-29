using Mealmap.Api.DataTransferObjects;
using Mealmap.Domain.DishAggregate;
using Mealmap.Domain.MealAggregate;

namespace Mealmap.Api.OutputMappers;

public static class OutputMapperRegistrationExtensions
{
    public static IServiceCollection AddOutputMappers(this IServiceCollection services)
    {
        return services
            .AddAutoMapper(typeof(AutomapperProfile))
            .AddScoped<IOutputMapper<DishDTO, Dish>, DishOutputMapper>()
            .AddScoped<IOutputMapper<MealDTO, Meal>, MealOutputMapper>();
    }
}
