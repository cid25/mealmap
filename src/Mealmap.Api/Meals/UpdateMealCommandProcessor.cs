using Mealmap.Api.Shared;
using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.Common.Validation;
using Mealmap.Domain.MealAggregate;

namespace Mealmap.Api.Meals;

public class UpdateMealCommandProcessor : ICommandProcessor<UpdateMealCommand, MealDTO>
{
    private readonly IMealRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutputMapper<MealDTO, Meal> _outputMapper;
    private readonly ILogger<UpdateMealCommandProcessor> _logger;
    private readonly MealDataTransferObjectValidator _validator;

    public UpdateMealCommandProcessor(
        IMealRepository repository,
        IUnitOfWork unitOfWork,
        IOutputMapper<MealDTO, Meal> outputMapper,
        ILogger<UpdateMealCommandProcessor> logger,
        MealDataTransferObjectValidator validator)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _outputMapper = outputMapper;
        _logger = logger;
        _validator = validator;
    }

    public async Task<CommandNotification<MealDTO>> Process(UpdateMealCommand command)
    {
        CommandNotification<MealDTO> notification = new();

        var meal = _repository.GetSingleById(command.Id);

        if (meal == null)
            return notification.WithNotFoundError("Meal with id not found.");

        if (_validator.Validate(command.Dto) is var validationResult && validationResult.Errors.Any())
            return notification.WithValidationErrorsFrom(validationResult);

        try
        {
            UpdatePropertiesFromRequest(meal, command);
        }
        catch (FormatException)
        {
            return notification.WithVersionMismatchError($"The version {command.Version} is not valid.");
        }

        _repository.Update(meal);

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

        _logger.LogInformation("Updated Meal with id {Id}", meal.Id);
        notification.Result = _outputMapper.FromEntity(meal);

        return notification;
    }

    private static void UpdatePropertiesFromRequest(Meal meal, UpdateMealCommand request)
    {
        meal.Version.Set(request.Version);
        meal.DiningDate = request.Dto.DiningDate;

        meal.RemoveAllCourses();
        if (request.Dto.Courses != null)
            foreach (var course in request.Dto.Courses)
                meal.AddCourse(course.Index, course.MainCourse, course.Attendees, course.DishId);
    }
}
