using Mealmap.Api.Common;

namespace Mealmap.Api.Meals;

public class UpdateMealCommand : TransferObjectCommand<MealDTO>
{
    public Guid Id { get; }

    public string Version { get; }

    public UpdateMealCommand(Guid id, string version, MealDTO dto) : base(dto)
    {
        (Id, Version) = (id, version);
    }
}
