using Mealmap.Api.Commands;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Api.OutputMappers;
using Mealmap.Domain.DishAggregate;
using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.Common.Validation;
using MediatR;

namespace Mealmap.Api.CommandHandlers;

public class CreateDishCommandHandler : IRequestHandler<CreateDishCommand, CommandNotification<DishDTO>>
{
    private readonly IDishRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutputMapper<DishDTO, Dish> _outputMapper;
    private readonly ILogger<CreateDishCommandHandler> _logger;

    public CreateDishCommandHandler(
        IDishRepository repository,
        IUnitOfWork unitOfWork,
        IOutputMapper<DishDTO, Dish> outputMapper,
        ILogger<CreateDishCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _outputMapper = outputMapper;
        _logger = logger;
    }

    public async Task<CommandNotification<DishDTO>> Handle(CreateDishCommand request, CancellationToken cancellationToken)
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
            return result;
        }

        _repository.Add(dish);

        try
        {
            await _unitOfWork.SaveTransactionAsync();
        }
        catch (DomainValidationException ex)
        {
            result.Errors.Add(new CommandError(CommandErrorCodes.NotValid, ex.Message));
            return result;
        }

        _logger.LogInformation("Created dish with id {Id}", dish.Id);
        result.Result = _outputMapper.FromEntity(dish);

        return result;
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
