using FluentValidation;
using Mealmap.Api.DataTransferObjects;
using Mealmap.Domain.DishAggregate;

namespace Mealmap.Api.DataTransferObjectValidators;

public class MealDataTransferObjectValidator : AbstractValidator<MealDTO>
{
    private readonly IDishRepository _dishRepository;

    public MealDataTransferObjectValidator(IDishRepository dishRepository)
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
                    .WithMessage("A given dish of a course does not exist."))
            .When(dto => dto.Courses != null);
    }
}
