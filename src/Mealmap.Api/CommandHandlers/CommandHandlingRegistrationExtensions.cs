using System.Reflection;

namespace Mealmap.Api.CommandHandlers;

public static class CommandHandlingRegistrationExtensions
{
    public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        return services.AddMediatR(cfg =>
        {
            cfg.AddOpenBehavior(typeof(CommandLogger<,>));
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
    }
}
