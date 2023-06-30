namespace Mealmap.Api.DataTransferObjects
{
    public class MealDto
    {
        public string Name { get; set; }

        public MealDto(string name)
        {
            Name = name;
        }
    }
}
