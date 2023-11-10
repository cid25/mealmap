using FluentValidation;
using Mealmap.Domain.Common.DataAccess;
using Mealmap.Domain.DishAggregate;

namespace Mealmap.Api.Meals;

public class MealDataTransferObjectValidator : AbstractValidator<MealDTO>
{
    private readonly IRepository<Dish> _dishRepository;

    public MealDataTransferObjectValidator(IRepository<Dish> dishRepository)
    {
        _dishRepository = dishRepository;

        RuleFor(dto => dto.Courses!.Where(x => x.MainCourse == true).Count())
            .LessThanOrEqualTo(1)
            .WithMessage("There may only be one main course.")
            .When(dto => dto.Courses != null);

        RuleForEach(dto => dto.Courses)
            .ChildRules(course =>
                course.RuleFor(c => c.DishId)
                    .Must(id => _dishRepository.GetSingleById(id) != null)
                    .WithMessage("The dish with id '{PropertyValue}' does not exist."))
            .When(dto => dto.Courses != null);
    }
}
