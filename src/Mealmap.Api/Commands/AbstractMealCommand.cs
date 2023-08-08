using Mealmap.Api.DataTransferObjects;

namespace Mealmap.Api.Commands;

public abstract class AbstractMealCommand
{
    public MealDTO Dto { get; }

    public AbstractMealCommand(MealDTO dto)
    {
        Dto = dto;
    }
}
