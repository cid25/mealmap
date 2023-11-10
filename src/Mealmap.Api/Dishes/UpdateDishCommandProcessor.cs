using Mealmap.Api.Shared;
using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.Common.Validation;
using Mealmap.Domain.DishAggregate;

namespace Mealmap.Api.Dishes;

public class UpdateDishCommandProcessor : ICommandProcessor<UpdateDishCommand, DishDTO>
{
    private readonly IRepository<Dish> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateDishCommandProcessor> _logger;
    private readonly IOutputMapper<DishDTO, Dish> _outputMapper;
    private readonly DishDataTransferObjectValidator _validator;

    public UpdateDishCommandProcessor(
        IRepository<Dish> repository,
        IUnitOfWork unitOfWork,
        IOutputMapper<DishDTO, Dish> outputMapper,
        ILogger<UpdateDishCommandProcessor> logger,
        DishDataTransferObjectValidator validator)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _outputMapper = outputMapper;
        _logger = logger;
        _validator = validator;
    }

    public async Task<CommandNotification<DishDTO>> Process(UpdateDishCommand command)
    {
        CommandNotification<DishDTO> notification = new();

        var dish = _repository.GetSingleById(command.Id);

        if (dish == null)
            return notification.WithNotFoundError("Dish with id not found.");

        if (_validator.Validate(command.Dto) is var validationResult && validationResult.Errors.Any())
            return notification.WithValidationErrorsFrom(validationResult);

        SetPropertiesFromRequest(dish, command);
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
        dish.Instructions = request.Dto.Instructions;

        dish.RemoveAllIngredients();
        if (request.Dto.Ingredients != null)
            foreach (var ing in request.Dto.Ingredients)
                dish.AddIngredient(ing.Quantity, ing.UnitOfMeasurement, ing.Description);
    }
}
