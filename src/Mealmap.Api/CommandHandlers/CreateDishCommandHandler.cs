using Mealmap.Api.Commands;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Api.OutputMappers;
using Mealmap.Domain.DishAggregate;
using Mealmap.Domain.Seedwork.Validation;
using MediatR;

namespace Mealmap.Api.CommandHandlers;

public class CreateDishCommandHandler : IRequestHandler<CreateDishCommand, CommandNotification<DishDTO>>
{
    private readonly IDishRepository _repository;
    private readonly IOutputMapper<DishDTO, Dish> _outputMapper;
    private readonly ILogger<CreateDishCommandHandler> _logger;

    public CreateDishCommandHandler(
        IDishRepository repository,
        IOutputMapper<DishDTO, Dish> outputMapper,
        ILogger<CreateDishCommandHandler> logger)
    {
        _repository = repository;
        _outputMapper = outputMapper;
        _logger = logger;
    }

    public Task<CommandNotification<DishDTO>> Handle(CreateDishCommand request, CancellationToken cancellationToken)
    {
        CommandNotification<DishDTO> result = new();

        Dish dish = new(request.Dto.Name);

        try
        {
            SetPropertiesFromRequest(dish, request);
        }
        catch (DomainValidationException ex)
        {
            result.Errors.Add(new CommandError(CommandErrorCodes.NotValid, ex.Message));
            return Task.FromResult(result);
        }

        _repository.Add(dish);

        _logger.LogInformation("Created dish with id {Id}", dish.Id);
        result.Result = _outputMapper.FromEntity(dish);

        return Task.FromResult(result);
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
