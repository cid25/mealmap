using Mealmap.Api.Commands;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Api.OutputMappers;
using Mealmap.Domain.MealAggregate;
using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.Common.Validation;
using MediatR;

namespace Mealmap.Api.CommandHandlers;

public class CreateMealCommandHandler : IRequestHandler<CreateMealCommand, CommandNotification<MealDTO>>
{
    private readonly IMealRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutputMapper<MealDTO, Meal> _outputMapper;
    private readonly ILogger<CreateMealCommandHandler> _logger;

    public CreateMealCommandHandler(
        IMealRepository repository,
        IUnitOfWork unitOfWork,
        IOutputMapper<MealDTO, Meal> outputMapper,
        ILogger<CreateMealCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _outputMapper = outputMapper;
        _logger = logger;
    }

    public async Task<CommandNotification<MealDTO>> Handle(CreateMealCommand request, CancellationToken cancellationToken)
    {
        CommandNotification<MealDTO> result = new();

        Meal meal = new(request.Dto.DiningDate);

        try
        {
            SetPropertiesFromRequest(meal, request);
        }
        catch (DomainValidationException ex)
        {
            result.Errors.Add(new CommandError(CommandErrorCodes.NotValid, ex.Message));
            return result;
        }

        _repository.Add(meal);

        try
        {
            await _unitOfWork.SaveTransactionAsync();
        }
        catch (DomainValidationException ex)
        {
            result.Errors.Add(new CommandError(CommandErrorCodes.NotValid, ex.Message));
            return result;
        }

        _logger.LogInformation("Created Meal with id {Id}", meal.Id);
        result.Result = _outputMapper.FromEntity(meal);

        return result;
    }

    /// <exception cref="DomainValidationException"/>
    private static void SetPropertiesFromRequest(Meal meal, CreateMealCommand request)
    {
        if (request.Dto.Courses != null)
            foreach (var course in request.Dto.Courses)
                meal.AddCourse(course.Index, course.MainCourse, course.DishId);
    }
}
