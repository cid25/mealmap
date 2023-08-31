using Mealmap.Api.Commands;

namespace Mealmap.Api.CommandValidators;

public static class CommandValidationRegistrationExtension
{
    public static IServiceCollection RegisterCommandValidation(this IServiceCollection services)
    {
        return services
            .AddScoped(typeof(MealCommandValidator))
            .AddScoped<ICommandValidator<UpdateMealCommand>, UpdateMealCommandValidator>()
            .AddScoped<ICommandValidator<CreateMealCommand>, CreateMealCommandValidator>();
    }
}
