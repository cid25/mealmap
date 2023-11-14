using Mealmap.Domain.Common.Validation;
using Mealmap.Domain.MealAggregate;

namespace Mealmap.Domain.UnitTests.MealAggregate;

public class MealTests
{
    private readonly Meal _meal;

    public MealTests()
    {
        _meal = new Meal(DateOnly.FromDateTime(DateTime.Now));
    }

    [Fact]
    public void AddCourse_WhenAddingSecondMainCourse_ThrowsDomainValidationException()
    {
        var course = (new TestCourseBuilder()).AsMainCourse().Build();
        _meal.AddCourse(course);

        var anotherMainCourse = (new TestCourseBuilder()).AsMainCourse().Build();
        Action act = () => _meal.AddCourse(anotherMainCourse);

        act.Should().Throw<DomainValidationException>();
    }

    [Fact]
    public void SetOrderOfCourses_WhenMultipleCoursesSameIndex_ShiftsCourse()
    {
        _meal.AddCourse((new TestCourseBuilder()).WithIndex(1).Build());
        _meal.AddCourse((new TestCourseBuilder()).WithIndex(2).Build());
        _meal.AddCourse((new TestCourseBuilder()).WithIndex(4).Build());

        _meal.AddCourse((new TestCourseBuilder()).WithIndex(2).Build());

        _meal.Courses.Where(x => x.Index == 2).Count().Should().Be(1);
        _meal.Courses.Where(x => x.Index == 3).Count().Should().Be(1);
        _meal.Courses.Where(x => x.Index == 4).Count().Should().Be(1);
    }

    [Fact]
    public void RemoveAllCourses_PurgesCourses()
    {
        _meal.AddCourse((new TestCourseBuilder()).Build());
        _meal.AddCourse((new TestCourseBuilder()).Build());
        _meal.AddCourse((new TestCourseBuilder()).Build());

        _meal.RemoveAllCourses();

        _meal.Courses.Should().HaveCount(0);
    }

    private class TestCourseBuilder
    {
        private int _index = 1;
        private bool _mainCourse = false;

        public TestCourseBuilder WithIndex(int index)
        {
            _index = index;
            return this;
        }

        public TestCourseBuilder AsMainCourse()
        {
            _mainCourse = true;
            return this;
        }

        public Course Build()
        {
            return new Course(index: _index, mainCourse: _mainCourse, attendees: 1, dishId: Guid.NewGuid());
        }
    }
}
