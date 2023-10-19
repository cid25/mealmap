using Mealmap.Domain.Common.Validation;
using Mealmap.Domain.MealAggregate;

namespace Mealmap.Domain.UnitTests.MealAggregate;

public class MealTests
{
    [Fact]
    public void AddCourse_WhenAddingSecondMainCourse_ThrowsDomainValidationException()
    {
        Meal meal = new(DateOnly.FromDateTime(DateTime.Now));
        meal.AddCourse(index: 1, mainCourse: true, attendees: 1, dishId: Guid.NewGuid());

        Action act = () => meal.AddCourse(index: 2, mainCourse: true, attendees: 1, dishId: Guid.NewGuid());

        act.Should().Throw<DomainValidationException>();
    }

    [Fact]
    public void SetOrderOfCourses_WhenMultipleCoursesSameIndex_ShiftsCourse()
    {
        var someDate = DateOnly.FromDateTime(DateTime.Now);
        var someDishId = Guid.NewGuid();
        var meal = new Meal(someDate);
        meal.AddCourse(index: 1, mainCourse: false, attendees: 1, dishId: someDishId);
        meal.AddCourse(index: 2, mainCourse: true, attendees: 1, dishId: someDishId);
        meal.AddCourse(index: 4, mainCourse: false, attendees: 1, dishId: someDishId);

        meal.AddCourse(index: 2, mainCourse: false, attendees: 1, dishId: someDishId);

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
        meal.AddCourse(index: 1, mainCourse: false, attendees: 1, dishId: someDishId);
        meal.AddCourse(index: 2, mainCourse: true, attendees: 1, dishId: someDishId);
        meal.AddCourse(index: 4, mainCourse: false, attendees: 1, dishId: someDishId);

        meal.RemoveAllCourses();

        meal.Courses.Should().HaveCount(0);
    }
}
