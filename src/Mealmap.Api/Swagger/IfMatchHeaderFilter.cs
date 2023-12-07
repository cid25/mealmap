using Mealmap.Api.Dishes;
using Mealmap.Api.Meals;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Mealmap.Api.Swagger;

public class IfMatchHeaderFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        List<string> applicableMethods =
        [
            nameof(DishesController.PutDish),
            nameof(MealsController.PutMeal),
        ];

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
