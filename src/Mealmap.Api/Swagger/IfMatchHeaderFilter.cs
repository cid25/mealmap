using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Mealmap.Api.Swagger;

public class IfMatchHeaderFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        List<string> applicableMethods = new()
        {
            nameof(Controllers.DishesController.PutDish),
        };

        if (applicableMethods.Contains(operation.OperationId))
        {
            operation.Parameters ??= new List<OpenApiParameter>();
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "If-Match",
                In = ParameterLocation.Header,
                Description = "The ETag of the entity",
                Required = true
            });
        }
    }
}
