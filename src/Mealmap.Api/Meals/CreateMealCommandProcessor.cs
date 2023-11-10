using Mealmap.Api.Shared;
using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.Common.Validation;
using Mealmap.Domain.MealAggregate;

namespace Mealmap.Api.Meals;

public class CreateMealCommandProcessor : ICommandProcessor<CreateMealCommand, MealDTO>
{
    private readonly IMealRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutputMapper<MealDTO, Meal> _outputMapper;
    private readonly ILogger<CreateMealCommandProcessor> _logger;
    private readonly MealDataTransferObjectValidator _validator;

    public CreateMealCommandProcessor(
        IMealRepository repository,
        IUnitOfWork unitOfWork,
        IOutputMapper<MealDTO, Meal> outputMapper,
        ILogger<CreateMealCommandProcessor> logger,
        MealDataTransferObjectValidator validator)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _outputMapper = outputMapper;
        _logger = logger;
        _validator = validator;
    }

    public async Task<CommandNotification<MealDTO>> Process(CreateMealCommand command)
    {
        CommandNotification<MealDTO> notification = new();

        if (_validator.Validate(command.Dto) is var validationResult && validationResult.Errors.Any())
            return notification.WithValidationErrorsFrom(validationResult);

        Meal meal = new(command.Dto.DiningDate);

        SetPropertiesFromRequest(meal, command);
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
                meal.AddCourse(course.Index, course.MainCourse, course.Attendees, course.DishId);
    }
}
