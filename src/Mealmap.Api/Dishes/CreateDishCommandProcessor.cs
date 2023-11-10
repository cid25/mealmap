using Mealmap.Api.Shared;
using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.Common.Validation;
using Mealmap.Domain.DishAggregate;

namespace Mealmap.Api.Dishes;

public class CreateDishCommandProcessor : ICommandProcessor<CreateDishCommand, DishDTO>
{
    private readonly IRepository<Dish> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutputMapper<DishDTO, Dish> _outputMapper;
    private readonly ILogger<CreateDishCommandProcessor> _logger;
    private readonly DishDataTransferObjectValidator _validator;

    public CreateDishCommandProcessor(
        IRepository<Dish> repository,
        IUnitOfWork unitOfWork,
        IOutputMapper<DishDTO, Dish> outputMapper,
        ILogger<CreateDishCommandProcessor> logger,
        DishDataTransferObjectValidator validator)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _outputMapper = outputMapper;
        _logger = logger;
        _validator = validator;
    }

    public async Task<CommandNotification<DishDTO>> Process(CreateDishCommand command)
    {
        CommandNotification<DishDTO> notification = new();

        if (_validator.Validate(command.Dto) is var validationResult && validationResult.Errors.Any())
            return notification.WithValidationErrorsFrom(validationResult);

        Dish dish = new(command.Dto.Name);

        try
        {
            SetPropertiesFromRequest(dish, command);
        }
        catch (DomainValidationException ex)
        {
            return notification.WithValidationError(ex.Message);
        }

        _repository.Add(dish);

        try
        {
            await _unitOfWork.SaveTransactionAsync();
        }
        catch (DomainValidationException ex)
        {
            return notification.WithValidationError(ex.Message);
        }

        _logger.LogInformation("Created dish with id {Id}", dish.Id);
        notification.Result = _outputMapper.FromEntity(dish);

        return notification;
    }

    private static void SetPropertiesFromRequest(Dish dish, CreateDishCommand request)
    {
        var dto = request.Dto;
        dish.Description = dto.Description;
        dish.Servings = dto.Servings;

        if (dto.Ingredients != null)
            foreach (var ing in dto.Ingredients)
                dish.AddIngredient(ing.Quantity, ing.UnitOfMeasurement, ing.Description);
    }
}
