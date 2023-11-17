using Mealmap.Api.Shared;
using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.Common.Validation;
using Mealmap.Domain.MealAggregate;

namespace Mealmap.Api.Meals;

public class UpdateMealCommandProcessor(
    IMealRepository repository,
    IUnitOfWork unitOfWork,
    IOutputMapper<MealDTO, Meal> outputMapper,
    ILogger<UpdateMealCommandProcessor> logger,
    MealDataTransferObjectValidator validator
)
    : ICommandProcessor<UpdateMealCommand, MealDTO>
{
    public async Task<CommandNotification<MealDTO>> Process(UpdateMealCommand command)
    {
        CommandNotification<MealDTO> notification = new();

        var meal = repository.GetSingleById(command.Id);

        if (meal == null)
            return notification.WithNotFoundError("Meal with id not found.");

        if (validator.Validate(command.Dto) is var validationResult && validationResult.Errors.Count != 0)
            return notification.WithValidationErrorsFrom(validationResult);

        try
        {
            UpdatePropertiesFromRequest(meal, command);
        }
        catch (FormatException)
        {
            return notification.WithVersionMismatchError($"The version {command.Version} is not valid.");
        }

        repository.Update(meal);

        try
        {
            await unitOfWork.SaveTransactionAsync();
        }
        catch (DomainValidationException ex)
        {
            return notification.WithValidationError(ex.Message);
        }
        catch (ConcurrentUpdateException)
        {
            return notification.WithVersionMismatchError();
        }

        logger.LogInformation("Updated Meal with id {Id}", meal.Id);
        notification.Result = outputMapper.FromEntity(meal);

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
