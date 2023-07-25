using Mealmap.Model;

namespace Mealmap.Api.DataTransfer
{
    public interface IDishMapper
    {
        Dish MapFromDTO(DishDTO dto);
        List<DishDTO> MapFromEntities(IEnumerable<Dish> dishes);
        DishDTO MapFromEntity(Dish dish);
    }
}