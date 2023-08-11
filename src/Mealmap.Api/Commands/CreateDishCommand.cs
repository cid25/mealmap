using Mealmap.Api.DataTransferObjects;
using MediatR;

namespace Mealmap.Api.Commands;

public class CreateDishCommand : AbstractCommand<DishDTO>, IRequest<CommandNotification<DishDTO>>
{
    public CreateDishCommand(DishDTO dto)
        : base(dto)
    {
        Dto = Dto with { Id = null };
    }
}
