using Mealmap.Api.DataTransferObjects;
using MediatR;

namespace Mealmap.Api.Commands;

public class UpdateDishCommand : IRequest<CommandNotification<DishDTO>>
{
    public Guid Id { get; }

    public string Version { get; }

    public DishDTO Dto { get; }

    public UpdateDishCommand(Guid id, string version, DishDTO dto)
    {
        (Id, Version, Dto) = (id, version, dto);
    }
}
