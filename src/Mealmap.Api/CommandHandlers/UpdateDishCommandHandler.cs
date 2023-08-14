using Mealmap.Api.Commands;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Api.OutputMappers;
using Mealmap.Domain;
using Mealmap.Domain.DishAggregate;
using Mealmap.Domain.Seedwork.Validation;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
        CommandNotification<DishDTO> result = new();

        var dish = _repository.GetSingleById(request.Id);

        if (dish == null)
        {
            result.Errors.Add(new CommandError(CommandErrorCodes.NotFound, "Dish with id not found."));
            return result;
        }

        setPropertiesFromRequest(dish, request);
        _repository.Update(dish);

        try
        {
            await _unitOfWork.SaveTransactionAsync();
        }
        catch (DomainValidationException ex)
        {
            result.Errors.Add(new CommandError(CommandErrorCodes.NotValid, ex.Message));
            return result;
        }
        catch (DbUpdateConcurrencyException)
        {
            result.Errors.Add(new CommandError(CommandErrorCodes.EtagMismatch, "If-Match Header does not match existing version."));
            return result;
        }

        _logger.LogInformation("Updated Dish with id {Id}", dish.Id);
        result.Result = _outputMapper.FromEntity(dish);

        return result;
    }

    private static void setPropertiesFromRequest(Dish dish, UpdateDishCommand request)
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
