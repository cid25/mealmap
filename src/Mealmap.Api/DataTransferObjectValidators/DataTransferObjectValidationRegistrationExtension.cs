using Mealmap.Api.DataTransferObjectValidators;

namespace Mealmap.Api.CommandValidators;

public static class DataTransferObjectValidationRegistrationExtension
{
    public static IServiceCollection AddDataTransferObjectValidators(this IServiceCollection services)
    {
        return services
            .AddScoped<MealDataTransferObjectValidator>()
            .AddScoped<DishDataTransferObjectValidator>();
    }
}
