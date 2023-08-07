using Mealmap.Api.DataTransferObjects;
using Mealmap.Api.OutputMappers;
using Mealmap.Domain.MealAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mealmap.Api.Commands;

public class UpdateMealCommandHandler : IRequestHandler<UpdateMealCommand, CommandNotification<MealDTO>>
{
    private readonly IMealRepository _repository;
    private readonly IOutputMapper<MealDTO, Meal> _outputMapper;
    private readonly ILogger<UpdateMealCommandHandler> _logger;
    private readonly IMealService _service;

    public UpdateMealCommandHandler(
        IMealRepository repository,
        IOutputMapper<MealDTO, Meal> outputMapper,
        ILogger<UpdateMealCommandHandler> logger,
        IMealService service)
    {
        _repository = repository;
        _outputMapper = outputMapper;
        _logger = logger;
        _service = service;
    }

    public Task<CommandNotification<MealDTO>> Handle(UpdateMealCommand request, CancellationToken cancellationToken)
    {
        CommandNotification<MealDTO> result = new();

        var meal = _repository.GetSingleById(request.Id);

        if (meal == null)
        {
            result.Errors.Add(new CommandError(CommandErrorCodes.NotFound, "Meal with id not found."));
            return Task.FromResult(result);
        }

        updatePropertiesFromRequest(meal, request);

        try
        {
            _repository.Update(meal);
        }
        catch (DbUpdateConcurrencyException)
        {
            result.Errors.Add(new CommandError(CommandErrorCodes.EtagMismatch, "If-Match Header does not match existing version."));
            return Task.FromResult(result);
        }

        _logger.LogInformation("Updated Meal with id {Id}", meal.Id);
        result.Result = _outputMapper.FromEntity(meal);

        return Task.FromResult(result);
    }

    private void updatePropertiesFromRequest(Meal meal, UpdateMealCommand request)
    {
        meal.Version.Set(request.Version);
        meal.DiningDate = request.Dto.DiningDate;

        meal.RemoveAllCourses();
        if (request.Dto.Courses != null)
            foreach (var course in request.Dto.Courses)
                _service.AddCourseToMeal(meal, course.Index, course.MainCourse, course.DishId);
    }
}
