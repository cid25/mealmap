using Mealmap.Domain.MealAggregate;
using Mealmap.Domain.Common.Validation;

namespace Mealmap.Domain.UnitTests.MealAggregate;

public class MealTests
{
    [Fact]
    public void AddCourse_WhenAddingSecondMainCourse_ThrowsDomainValidationException()
    {
        Meal meal = new(DateOnly.FromDateTime(DateTime.Now));
        meal.AddCourse(1, true, Guid.NewGuid());

        Action act = () => meal.AddCourse(1, true, Guid.NewGuid());

        act.Should().Throw<DomainValidationException>();
    }

    [Fact]
    public void SetOrderOfCourses_WhenMultipleCoursesSameIndex_ShiftsCourse()
    {
        var someDate = DateOnly.FromDateTime(DateTime.Now);
        var someDishId = Guid.NewGuid();
        var meal = new Meal(someDate);
        meal.AddCourse(1, false, someDishId);
        meal.AddCourse(2, true, someDishId);
        meal.AddCourse(4, false, someDishId);

        meal.AddCourse(2, false, someDishId);

        meal.Courses.Where(x => x.Index == 2).Count().Should().Be(1);
        meal.Courses.Where(x => x.Index == 3).Count().Should().Be(1);
        meal.Courses.Where(x => x.Index == 4).Count().Should().Be(1);
    }

    [Fact]
    public void RemoveAllCourses_PurgesCourses()
    {
        var someDate = DateOnly.FromDateTime(DateTime.Now);
        var someDishId = Guid.NewGuid();
        var meal = new Meal(someDate);
        meal.AddCourse(1, false, someDishId);
        meal.AddCourse(2, true, someDishId);
        meal.AddCourse(4, false, someDishId);

        meal.RemoveAllCourses();

        meal.Courses.Should().HaveCount(0);
    }
}
