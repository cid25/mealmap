using Mealmap.Api.DataTransferObjects;
using MediatR;

namespace Mealmap.Api.Commands;

public class CreateMealCommand : AbstractMealCommand, IRequest<CommandNotification<MealDTO>>
{
    public CreateMealCommand(MealDTO dto) : base(dto) { }
}
