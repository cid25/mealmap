using Mealmap.Api.Shared;

namespace Mealmap.Api.Dishes;

public class CreateDishCommand : TransferObjectCommand<DishDTO>
{
    public CreateDishCommand(DishDTO dto)
        : base(dto)
    {
        Dto = Dto with { Id = null };
    }
}
