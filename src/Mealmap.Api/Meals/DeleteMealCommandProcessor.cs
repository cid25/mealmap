using Mealmap.Api.Dishes;
using Mealmap.Api.Shared;
using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.MealAggregate;

namespace Mealmap.Api.Meals;

public class DeleteMealCommandProcessor : ICommandProcessor<DeleteMealCommand, MealDTO>
{
    private readonly IMealRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutputMapper<MealDTO, Meal> _outputMapper;

    public DeleteMealCommandProcessor(IMealRepository repository, IUnitOfWork unitOfWork, IOutputMapper<MealDTO, Meal> outputMapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _outputMapper = outputMapper;
    }

    public async Task<CommandNotification<MealDTO>> Process(DeleteMealCommand command)
    {
        CommandNotification<MealDTO> notification = new();

        var meal = _repository.GetSingleById(command.Id);

        if (meal == null)
            return notification.WithNotFoundError("Meal with id not found.");

        _repository.Remove(meal);
        await _unitOfWork.SaveTransactionAsync();

        notification.Result = _outputMapper.FromEntity(meal);
        return notification;
    }
}
