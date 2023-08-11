using Mealmap.Api.DataTransferObjects;
using Mealmap.Api.OutputMappers;
using Mealmap.Domain;
using Mealmap.Domain.MealAggregate;
using Mealmap.Domain.Seedwork.Validation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mealmap.Api.Commands;

public class UpdateMealCommandHandler : IRequestHandler<UpdateMealCommand, CommandNotification<MealDTO>>
{
    private readonly IMealRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutputMapper<MealDTO, Meal> _outputMapper;
    private readonly ILogger<UpdateMealCommandHandler> _logger;

    public UpdateMealCommandHandler(
        IMealRepository repository,
        IUnitOfWork unitOfWork,
        IOutputMapper<MealDTO, Meal> outputMapper,
        ILogger<UpdateMealCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _outputMapper = outputMapper;
        _logger = logger;
    }

    public async Task<CommandNotification<MealDTO>> Handle(UpdateMealCommand request, CancellationToken cancellationToken)
    {
        CommandNotification<MealDTO> result = new();

        var meal = _repository.GetSingleById(request.Id);

        if (meal == null)
        {
            result.Errors.Add(new CommandError(CommandErrorCodes.NotFound, "Meal with id not found."));
            return result;
        }

        updatePropertiesFromRequest(meal, request);
        _repository.Update(meal);

        try
        {
            await _unitOfWork.SaveTransactionAsync();
        }
        catch (DomainValidationException)
        {
            result.Errors.Add(new CommandError(CommandErrorCodes.NotValid, "The request is not valid."));
            return result;
        }
        catch (DbUpdateConcurrencyException)
        {
            result.Errors.Add(new CommandError(CommandErrorCodes.EtagMismatch, "If-Match Header does not match existing version."));
            return result;
        }

        _logger.LogInformation("Updated Meal with id {Id}", meal.Id);
        result.Result = _outputMapper.FromEntity(meal);

        return result;
    }

    private static void updatePropertiesFromRequest(Meal meal, UpdateMealCommand request)
    {
        meal.Version.Set(request.Version);
        meal.DiningDate = request.Dto.DiningDate;

        meal.RemoveAllCourses();
        if (request.Dto.Courses != null)
            foreach (var course in request.Dto.Courses)
                meal.AddCourse(course.Index, course.MainCourse, course.DishId);
    }
}
