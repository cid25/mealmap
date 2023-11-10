using Mealmap.Api.Shared;
using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.DishAggregate;

namespace Mealmap.Api.Dishes;

public class DeleteDishImageCommandProcessor : ICommandProcessor<DeleteDishImageCommand, DishDTO>
{
    private readonly IRepository<Dish> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutputMapper<DishDTO, Dish> _outputMapper;

    public DeleteDishImageCommandProcessor(IRepository<Dish> repository, IUnitOfWork unitOfWork, IOutputMapper<DishDTO, Dish> outputMapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _outputMapper = outputMapper;
    }

    public async Task<CommandNotification<DishDTO>> Process(DeleteDishImageCommand command)
    {
        CommandNotification<DishDTO> notification = new();

        var dish = _repository.GetSingleById(command.Id);

        if (dish == null) return notification.WithNotFoundError("Dish with id not found.");

        if (dish.Image == null) return notification.WithNotFoundError("Image not found.");

        dish.RemoveImage();
        _repository.Update(dish);

        await _unitOfWork.SaveTransactionAsync();

        notification.Result = _outputMapper.FromEntity(dish);
        return notification;
    }
}
