using System.ComponentModel.DataAnnotations;

namespace Mealmap.Api.DataTransfer
{
    public record DishDTO
    {
        public Guid? Id { get; init; }

        [Required]
        public string Name { get; init; }

        public DishDTO(string name)
        {
            Name = name;
        }
    }
}
