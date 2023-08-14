using Mealmap.Api.DataTransferObjects;
using MediatR;

namespace Mealmap.Api.Commands;

public class UpdateMealCommand : AbstractCommand<MealDTO>, IRequest<CommandNotification<MealDTO>>
{
    public Guid Id { get; }

    public string Version { get; }

    public UpdateMealCommand(Guid id, string version, MealDTO dto) : base(dto)
    {
        (Id, Version) = (id, version);
    }
}
