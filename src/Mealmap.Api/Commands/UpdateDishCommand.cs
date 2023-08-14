using Mealmap.Api.DataTransferObjects;
using MediatR;

namespace Mealmap.Api.Commands;

public class UpdateDishCommand : AbstractCommand<DishDTO>, IRequest<CommandNotification<DishDTO>>
{
    public Guid Id { get; }

    public string Version { get; }

    public UpdateDishCommand(Guid id, string version, DishDTO dto)
        : base(dto)
    {
        (Id, Version) = (id, version);
    }
}
