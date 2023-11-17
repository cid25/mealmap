using Mealmap.Api.Shared;
using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.Common.Validation;
using Mealmap.Domain.DishAggregate;

namespace Mealmap.Api.Dishes;

public class UpdateDishImageCommandProcessor(IRepository<Dish> repository, IUnitOfWork unitOfWork, IOutputMapper<DishDTO, Dish> outputMapper)
    : ICommandProcessor<UpdateDishImageCommand, DishDTO>
{
    public async Task<CommandNotification<DishDTO>> Process(UpdateDishImageCommand command)
    {
        CommandNotification<DishDTO> notification = new();

        var dish = repository.GetSingleById(command.Id);

        if (dish == null) return notification.WithNotFoundError("Dish with id not found.");

        dish.SetImage(command.Image.Content, command.Image.ContentType);
        repository.Update(dish);

        try
        {
            await unitOfWork.SaveTransactionAsync();
        }
        catch (DomainValidationException)
        {
            return notification.WithValidationError(String.Empty);
        }

        notification.Result = outputMapper.FromEntity(dish);
        return notification;
    }
}
