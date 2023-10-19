using Mealmap.Api.Commands;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Api.DataTransferObjectValidators;
using Mealmap.Api.OutputMappers;
using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.Common.Validation;
using Mealmap.Domain.MealAggregate;
using MediatR;

namespace Mealmap.Api.CommandHandlers;

public class UpdateMealCommandHandler : IRequestHandler<UpdateMealCommand, CommandNotification<MealDTO>>
{
    private readonly IMealRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutputMapper<MealDTO, Meal> _outputMapper;
    private readonly ILogger<UpdateMealCommandHandler> _logger;
    private readonly MealDataTransferObjectValidator _validator;

    public UpdateMealCommandHandler(
        IMealRepository repository,
        IUnitOfWork unitOfWork,
        IOutputMapper<MealDTO, Meal> outputMapper,
        ILogger<UpdateMealCommandHandler> logger,
        MealDataTransferObjectValidator validator)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _outputMapper = outputMapper;
        _logger = logger;
        _validator = validator;
    }

    public async Task<CommandNotification<MealDTO>> Handle(UpdateMealCommand request, CancellationToken cancellationToken)
    {
        CommandNotification<MealDTO> notification = new();

        var meal = _repository.GetSingleById(request.Id);

        if (meal == null)
            return notification.WithNotFoundError("Meal with id not found.");

        if (_validator.Validate(request.Dto) is var validationResult && validationResult.Errors.Any())
            return notification.WithValidationErrorsFrom(validationResult);

        UpdatePropertiesFromRequest(meal, request);
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
