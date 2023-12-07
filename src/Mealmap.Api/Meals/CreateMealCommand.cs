using Mealmap.Api.Common;

namespace Mealmap.Api.Meals;

public class CreateMealCommand : TransferObjectCommand<MealDTO>
{
    public CreateMealCommand(MealDTO dto) : base(dto)
    {
        Dto = Dto with { Id = null };
    }
}
