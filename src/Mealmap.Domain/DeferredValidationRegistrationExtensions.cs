using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Mealmap.Domain;

public static class DeferredValidationRegistrationExtensions
{
    public static void RegisterDeferredDomainValidation(this MediatRServiceConfiguration configuration)
    {
        configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    }
}
