using Mealmap.Api.Common;
using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.Common.Validation;
using Mealmap.Domain.DishAggregate;

namespace Mealmap.Api.Dishes;

public class CreateDishCommandProcessor(
    IRepository<Dish> repository,
    IUnitOfWork unitOfWork,
    IOutputMapper<DishDTO, Dish> outputMapper,
    ILogger<CreateDishCommandProcessor> logger,
    DishDataTransferObjectValidator validator) : ICommandProcessor<CreateDishCommand, DishDTO>
{
    public async Task<CommandNotification<DishDTO>> Process(CreateDishCommand command)
    {
        CommandNotification<DishDTO> notification = new();

        if (validator.Validate(command.Dto) is var validationResult && validationResult.Errors.Count != 0)
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

        repository.Add(dish);

        try
        {
            await unitOfWork.SaveTransactionAsync();
        }
        catch (DomainValidationException ex)
        {
            return notification.WithValidationError(ex.Message);
        }

        logger.LogInformation("Created dish with id {Id}", dish.Id);
        notification.Result = outputMapper.FromEntity(dish);

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
