using Mealmap.Domain.DishAggregate;
using Mealmap.Domain.Seedwork.Validation;
using MediatR;

namespace Mealmap.Domain.MealAggregate;

public class MealValidator : IRequestHandler<ValidationRequest<Meal>, DomainValidationResult>
{
    private readonly IDishRepository _repository;

    public MealValidator(IDishRepository repository)
    {
        _repository = repository;
    }

    public Task<DomainValidationResult> Handle(ValidationRequest<Meal> request, CancellationToken cancellationToken)
    {
        DomainValidationResult result = new();
        var meal = request.Entity;

        if (meal.Courses.Any())
            foreach (var course in meal.Courses)
                if (_repository.GetSingleById(course.DishId) == null)
                    result.AddError($"Dish with Id {course.DishId} not found.");

        return Task.FromResult(result);
    }
}
