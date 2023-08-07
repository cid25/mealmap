using Mealmap.Api.DataTransferObjects;
using MediatR;

namespace Mealmap.Api.Commands;

public class UpdateMealCommand : IRequest<CommandNotification<MealDTO>>
{
    public Guid Id { get; }

    public string Version { get; }

    public MealDTO Dto { get; }

    public UpdateMealCommand(Guid id, string version, MealDTO dto)
    {
        (Id, Version, Dto) = (id, version, dto);
    }
}
