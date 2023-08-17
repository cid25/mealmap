using Mealmap.Api.Commands;

namespace Mealmap.Api.CommandValidators;

public static class CommandValidationRegistrationExtension
{
    public static IServiceCollection RegisterCommandValidation(this IServiceCollection services)
    {
        return services
            .AddScoped(typeof(MealCommandValidations))
            .AddScoped<ICommandValidator<UpdateMealCommand>, UpdateMealCommandValidator>()
            .AddScoped<ICommandValidator<CreateMealCommand>, CreateMealCommandValidator>();
    }
}
