using Mealmap.Api.DataTransfer;
using Swashbuckle.AspNetCore.Filters;


namespace Mealmap.Api.Swashbuckle
{
    public class MealPostRequestExample : IExamplesProvider<MealDTO>
    {
        public MealDTO GetExamples()
        {
            return new MealDTO()
            {
                DiningDate = DateOnly.FromDateTime(DateTime.Now),
                DishId = Guid.NewGuid(),
            };
        }
    }
}
