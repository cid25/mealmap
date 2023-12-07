using Mealmap.Api.Common;
using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.DishAggregate;

namespace Mealmap.Api.Dishes;

public class DeleteDishImageCommandProcessor(IRepository<Dish> repository, IUnitOfWork unitOfWork, IOutputMapper<DishDTO, Dish> outputMapper)
    : ICommandProcessor<DeleteDishImageCommand, DishDTO>
{
    public async Task<CommandNotification<DishDTO>> Process(DeleteDishImageCommand command)
    {
        CommandNotification<DishDTO> notification = new();

        var dish = repository.GetSingleById(command.Id);

        if (dish == null) return notification.WithNotFoundError("Dish with id not found.");

        if (dish.Image == null) return notification.WithNotFoundError("Image not found.");

        dish.RemoveImage();
        repository.Update(dish);

        await unitOfWork.SaveTransactionAsync();

        notification.Result = outputMapper.FromEntity(dish);
        return notification;
    }
}
