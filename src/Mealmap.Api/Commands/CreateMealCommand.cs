using Mealmap.Api.DataTransferObjects;
using MediatR;

namespace Mealmap.Api.Commands;

public class CreateMealCommand : AbstractCommand<MealDTO>, IRequest<CommandNotification<MealDTO>>
{
    public CreateMealCommand(MealDTO dto) : base(dto)
    {
        Dto = Dto with { Id = null };
    }
}
