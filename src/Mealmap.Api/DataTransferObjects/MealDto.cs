using System.ComponentModel.DataAnnotations;

namespace Mealmap.Api.DataTransferObjects
{
    public record MealDTO
    {
        public Guid? Id { get; init; }

        [Required]
        public string Name { get; init; }

        public MealDTO(string name)
        {
            Name = name;
        }
    }
}
