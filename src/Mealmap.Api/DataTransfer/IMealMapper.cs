using Mealmap.Model;

namespace Mealmap.Api.DataTransfer
{
    public interface IMealMapper
    {
        Meal MapFromDTO(MealDTO dto);
        List<MealDTO> MapFromEntities(IEnumerable<Meal> meals);
        MealDTO MapFromEntity(Meal meal);
    }
}