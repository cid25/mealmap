using Mealmap.Api.Commands;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Api.DataTransferObjectValidators;
using Mealmap.Api.OutputMappers;
using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.Common.Validation;
using Mealmap.Domain.MealAggregate;
using MediatR;

namespace Mealmap.Api.CommandHandlers;

public class CreateMealCommandHandler : IRequestHandler<CreateMealCommand, CommandNotification<MealDTO>>
{
    private readonly IMealRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutputMapper<MealDTO, Meal> _outputMapper;
    private readonly ILogger<CreateMealCommandHandler> _logger;
    private readonly MealDataTransferObjectValidator _validator;

    public CreateMealCommandHandler(
        IMealRepository repository,
        IUnitOfWork unitOfWork,
        IOutputMapper<MealDTO, Meal> outputMapper,
        ILogger<CreateMealCommandHandler> logger,
        MealDataTransferObjectValidator validator)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _outputMapper = outputMapper;
        _logger = logger;
        _validator = validator;
    }

    public async Task<CommandNotification<MealDTO>> Handle(CreateMealCommand request, CancellationToken cancellationToken)
    {
        CommandNotification<MealDTO> notification = new();

        if (_validator.Validate(request.Dto) is var validationResult && validationResult.Errors.Any())
            return notification.WithValidationErrorsFrom(validationResult);

        Meal meal = new(request.Dto.DiningDate);

        SetPropertiesFromRequest(meal, request);
        _repository.Add(meal);

        try
        {
            await _unitOfWork.SaveTransactionAsync();
        }
        catch (DomainValidationException ex)
        {
            return notification.WithValidationError(ex.Message);
        }

        _logger.LogInformation("Created Meal with id {Id}", meal.Id);
        notification.Result = _outputMapper.FromEntity(meal);

        return notification;
    }

    private static void SetPropertiesFromRequest(Meal meal, CreateMealCommand request)
    {
        if (request.Dto.Courses != null)
            foreach (var course in request.Dto.Courses)
                meal.AddCourse(course.Index, course.MainCourse, course.DishId);
    }
}
