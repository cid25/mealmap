using Mealmap.Api.Commands;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Domain.DishAggregate;
using MediatR;

namespace Mealmap.Api.Behaviors;

public class MealCommandValidationBehavior : IPipelineBehavior<AbstractCommand<MealDTO>, CommandNotification<MealDTO>>
{
    private readonly IDishRepository _repository;

    public MealCommandValidationBehavior(IDishRepository repository)
        => _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public Task<CommandNotification<MealDTO>> Handle(AbstractCommand<MealDTO> request, RequestHandlerDelegate<CommandNotification<MealDTO>> next, CancellationToken cancellationToken)
    {
        if (request.Dto.Courses != null)
        {
            CommandNotification<MealDTO> result = new();

            request.Dto.Courses
                .Select(c => new { Exists = _repository.GetSingleById(c.DishId) != null, Id = c.DishId })
                .Where(c => !c.Exists)
                .ToList()
                .ForEach(f => result.Errors.Add(
                    new CommandError(CommandErrorCodes.NotValid, $"Dish with id {f.Id} not found.")));

            if (result.Errors.Count > 0)
                return Task.FromResult(result);
        }

        return next();
    }
}
