using Mealmap.Api.DataTransfer;
using Swashbuckle.AspNetCore.Filters;


namespace Mealmap.Api.Swashbuckle
{
    public class DishPostResponseExample : IExamplesProvider<DishDTO>
    {
        public DishDTO GetExamples()
        {
            return new DishDTO(name: "Pineapple Pizza")
            {
                Id = Guid.NewGuid(),
                Description = "An italian-style pizza with pineapple and mozarella.",
                Servings = 2,
                Ingredients = new IngredientDTO[] {
                    new IngredientDTO(150, "Gram", "Flour"),
                    new IngredientDTO(260, "Gram", "Sugar"),
                    new IngredientDTO(100, "Gram", "Butter"),
                    new IngredientDTO(600, "Mililiter", "Milk"),
                    new IngredientDTO(2, "Piece", "Eggs"),
                    new IngredientDTO(200, "Mililiter", "Oil"),
                }
            };
        }
    }
}
