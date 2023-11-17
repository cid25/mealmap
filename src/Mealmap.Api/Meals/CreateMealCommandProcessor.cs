using Mealmap.Api.Common;
using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.Common.Validation;
using Mealmap.Domain.MealAggregate;

namespace Mealmap.Api.Meals;

public class CreateMealCommandProcessor(
    IMealRepository repository,
    IUnitOfWork unitOfWork,
    IOutputMapper<MealDTO, Meal> outputMapper,
    ILogger<CreateMealCommandProcessor> logger,
    MealDataTransferObjectValidator validator
)
    : ICommandProcessor<CreateMealCommand, MealDTO>
{
    public async Task<CommandNotification<MealDTO>> Process(CreateMealCommand command)
    {
        CommandNotification<MealDTO> notification = new();

        if (validator.Validate(command.Dto) is var validationResult && validationResult.Errors.Count != 0)
            return notification.WithValidationErrorsFrom(validationResult);

        Meal meal = new(command.Dto.DiningDate);

        SetPropertiesFromRequest(meal, command);
        repository.Add(meal);

        try
        {
            await unitOfWork.SaveTransactionAsync();
        }
        catch (DomainValidationException ex)
        {
            return notification.WithValidationError(ex.Message);
        }

        logger.LogInformation("Created Meal with id {Id}", meal.Id);
        notification.Result = outputMapper.FromEntity(meal);

        return notification;
    }

    private static void SetPropertiesFromRequest(Meal meal, CreateMealCommand request)
    {
        if (request.Dto.Courses != null)
            foreach (var course in request.Dto.Courses)
                meal.AddCourse(course.Index, course.MainCourse, course.Attendees, course.DishId);
    }
}
