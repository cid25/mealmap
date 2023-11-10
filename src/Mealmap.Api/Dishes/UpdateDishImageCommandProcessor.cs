using Mealmap.Api.Shared;
using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.Common.Validation;
using Mealmap.Domain.DishAggregate;

namespace Mealmap.Api.Dishes;

public class UpdateDishImageCommandProcessor : ICommandProcessor<UpdateDishImageCommand, DishDTO>
{
    private readonly IRepository<Dish> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutputMapper<DishDTO, Dish> _outputMapper;

    public UpdateDishImageCommandProcessor(IRepository<Dish> repository, IUnitOfWork unitOfWork, IOutputMapper<DishDTO, Dish> outputMapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _outputMapper = outputMapper;
    }

    public async Task<CommandNotification<DishDTO>> Process(UpdateDishImageCommand command)
    {
        CommandNotification<DishDTO> notification = new();

        var dish = _repository.GetSingleById(command.Id);

        if (dish == null) return notification.WithNotFoundError("Dish with id not found.");

        dish.SetImage(command.Image.Content, command.Image.ContentType);
        _repository.Update(dish);

        try
        {
            await _unitOfWork.SaveTransactionAsync();
        }
        catch (DomainValidationException)
        {
            return notification.WithValidationError(String.Empty);
        }

        notification.Result = _outputMapper.FromEntity(dish);
        return notification;
    }
}
