using Mealmap.Api.Dishes;
using Mealmap.Api.Common;
using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.MealAggregate;

namespace Mealmap.Api.Meals;

public class DeleteMealCommandProcessor(IMealRepository repository, IUnitOfWork unitOfWork, IOutputMapper<MealDTO, Meal> outputMapper)
    : ICommandProcessor<DeleteMealCommand, MealDTO>
{
    public async Task<CommandNotification<MealDTO>> Process(DeleteMealCommand command)
    {
        CommandNotification<MealDTO> notification = new();

        var meal = repository.GetSingleById(command.Id);

        if (meal == null)
            return notification.WithNotFoundError("Meal with id not found.");

        repository.Remove(meal);
        await unitOfWork.SaveTransactionAsync();

        notification.Result = outputMapper.FromEntity(meal);
        return notification;
    }
}
