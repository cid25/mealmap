using Microsoft.Extensions.DependencyInjection;

namespace Mealmap.Domain.Common.Validation;

public static class DeferredValidationRegistrationExtensions
{
    public static void RegisterDeferredDomainValidation(this IServiceCollection services)
    {
        services.AddScoped<IDeferredDomainValidator, DeferredDomainValidator>();

        services.Scan(scan => scan
            .FromCallingAssembly()
                .AddClasses(classes => classes.AssignableTo(typeof(AbstractEntityValidator<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());
    }
}
