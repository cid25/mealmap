using Mealmap.Api.DataTransferObjects;
using Swashbuckle.AspNetCore.Filters;

namespace Mealmap.Api.Swagger;

public class DishRequestExampleWithIdWithoutEtag : IExamplesProvider<DishDTO>
{
    public DishDTO GetExamples()
    {
        return new DishDTO(name: "Pineapple Pizza")
        {
            Id = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
            Description = "An italian-style pizza with pineapple and mozarella.",
            Servings = 2,
            Ingredients = new IngredientDTO[] {
                new IngredientDTO(150, "Gram", "Flour"),
                new IngredientDTO(260, "Gram", "Sugar"),
                new IngredientDTO(100, "Gram", "Butter"),
                new IngredientDTO(600, "Mililiter", "Milk"),
                new IngredientDTO(2, "Piece", "Eggs"),
                new IngredientDTO(200, "Mililiter", "Oil"),
            },
            Instructions = """
            Mix the ingredients together to form a firm dough.

            Roll out the dough and add toppings.
            """
        };
    }
}
