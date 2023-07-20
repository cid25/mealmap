using Mealmap.Api.DataTransfer;
using Swashbuckle.AspNetCore.Filters;


namespace Mealmap.Api.Swashbuckle
{
    public class MealPostResponseExample : IExamplesProvider<MealDTO>
    {
        public MealDTO GetExamples()
        {
            return new MealDTO()
            {
                Id = Guid.NewGuid(),
                DiningDate = DateOnly.FromDateTime(DateTime.Now),
                DishId = Guid.NewGuid(),
            };
        }
    }
}
