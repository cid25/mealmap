using Mealmap.Api.Shared;

namespace Mealmap.Api.Dishes;

public class UpdateDishCommand : TransferObjectCommand<DishDTO>
{
    public Guid Id { get; }

    public string Version { get; }

    public UpdateDishCommand(Guid id, string version, DishDTO dto)
        : base(dto)
    {
        (Id, Version) = (id, version);
    }
}
