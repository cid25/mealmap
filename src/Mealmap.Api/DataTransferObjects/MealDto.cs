namespace Mealmap.Api.DataTransferObjects
{
    public class MealDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public MealDto(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
