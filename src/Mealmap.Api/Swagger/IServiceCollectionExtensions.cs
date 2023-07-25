using System.Reflection;
using Swashbuckle.AspNetCore.Filters;

namespace Mealmap.Api.Swagger
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddSwaggerOperationExamples(this IServiceCollection services)
            => services.AddSwaggerExamplesFromAssemblies(Assembly.GetAssembly(typeof(IServiceCollectionExtensions)));
    }
}
