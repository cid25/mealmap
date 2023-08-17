using Mealmap.Api.Commands;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Api.OutputMappers;
using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.Common.Validation;
using Mealmap.Domain.DishAggregate;
using MediatR;

namespace Mealmap.Api.CommandHandlers;

public class UpdateDishCommandHandler : IRequestHandler<UpdateDishCommand, CommandNotification<DishDTO>>
{
    private readonly IDishRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateDishCommandHandler> _logger;
    private readonly IOutputMapper<DishDTO, Dish> _outputMapper;

    public UpdateDishCommandHandler(
        IDishRepository repository,
        IUnitOfWork unitOfWork,
        IOutputMapper<DishDTO, Dish> outputMapper,
        ILogger<UpdateDishCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _outputMapper = outputMapper;
        _logger = logger;
    }

    public async Task<CommandNotification<DishDTO>> Handle(UpdateDishCommand request, CancellationToken cancellationToken)
    {
        CommandNotification<DishDTO> notification = new();

        var dish = _repository.GetSingleById(request.Id);

        if (dish == null)
            return notification.WithNotFoundError("Dish with id not found.");

        SetPropertiesFromRequest(dish, request);
        _repository.Update(dish);

        try
        {
            await _unitOfWork.SaveTransactionAsync();
        }
        catch (DomainValidationException ex)
        {
            return notification.WithValidationError(ex.Message);
        }
        catch (ConcurrentUpdateException)
        {
            return notification.WithVersionMismatchError();
        }

        _logger.LogInformation("Updated Dish with id {Id}", dish.Id);
        notification.Result = _outputMapper.FromEntity(dish);

        return notification;
    }

    private static void SetPropertiesFromRequest(Dish dish, UpdateDishCommand request)
    {
        dish.Version.Set(request.Version);
        dish.Name = request.Dto.Name;
        dish.Description = request.Dto.Description;
        dish.Servings = request.Dto.Servings;

        dish.RemoveAllIngredients();
        if (request.Dto.Ingredients != null)
            foreach (var ing in request.Dto.Ingredients)
                dish.AddIngredient(ing.Quantity, ing.UnitOfMeasurement, ing.Description);
    }
}
