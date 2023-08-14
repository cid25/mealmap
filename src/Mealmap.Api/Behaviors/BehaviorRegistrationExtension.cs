using Mealmap.Api.Commands;
using Mealmap.Api.DataTransferObjects;
using MediatR;

namespace Mealmap.Api.Behaviors;

public static class BehaviorRegistrationExtension
{
    public static void RegisterApplicationBehaviors(this MediatRServiceConfiguration configuration)
    {
        configuration.AddBehavior<IPipelineBehavior<UpdateMealCommand, CommandNotification<MealDTO>>, MealCommandValidationBehavior>();
        configuration.AddBehavior<IPipelineBehavior<CreateMealCommand, CommandNotification<MealDTO>>, MealCommandValidationBehavior>();
    }
}
