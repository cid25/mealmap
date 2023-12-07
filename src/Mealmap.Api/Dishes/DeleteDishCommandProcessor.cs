using Mealmap.Api.Common;
using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.DishAggregate;

namespace Mealmap.Api.Dishes;

public class DeleteDishCommandProcessor(IRepository<Dish> repository, IUnitOfWork unitOfWork, IOutputMapper<DishDTO, Dish> outputMapper)
    : ICommandProcessor<DeleteDishCommand, DishDTO>
{
    public async Task<CommandNotification<DishDTO>> Process(DeleteDishCommand command)
    {
        CommandNotification<DishDTO> notification = new();

        var dish = repository.GetSingleById(command.Id);

        if (dish == null)
            return notification.WithNotFoundError("Dish with id not found.");

        repository.Remove(dish);
        await unitOfWork.SaveTransactionAsync();

        notification.Result = outputMapper.FromEntity(dish);
        return notification;
    }
}
