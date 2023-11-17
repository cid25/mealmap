using Mealmap.Api.Dishes;
using Swashbuckle.AspNetCore.Filters;

namespace Mealmap.Api.Swagger;

public class DishResponseExampleWithIdAndEtag : IExamplesProvider<DishDTO>
{
    public DishDTO GetExamples()
    {
        return new DishDTO(name: "Pineapple Pizza")
        {
            Id = Guid.NewGuid(),
            ETag = "AAAAAAAAB9E=",
            Description = "An italian-style pizza with pineapple and mozarella.",
            Servings = 2,
            Ingredients = [
                new IngredientDTO(150, "Gram", "Flour"),
                new IngredientDTO(260, "Gram", "Sugar"),
                new IngredientDTO(100, "Gram", "Butter"),
                new IngredientDTO(600, "Mililiter", "Milk"),
                new IngredientDTO(2, "Piece", "Eggs"),
                new IngredientDTO(200, "Mililiter", "Oil"),
            ],
            Instructions = """
            Mix the ingredients together to form a firm dough.

            Roll out the dough and add toppings.
            """
        };
    }
}
