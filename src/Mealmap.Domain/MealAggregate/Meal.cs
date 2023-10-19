using Mealmap.Domain.Common;
using Mealmap.Domain.Common.Validation;

namespace Mealmap.Domain.MealAggregate;

public class Meal : EntityBase
{
    public DateOnly DiningDate { get; set; }

    private List<Course> _courses = new();

    public IReadOnlyCollection<Course> Courses
    {
        get => _courses.AsReadOnly();
    }

    public Meal(DateOnly diningDate) : base()
    {
        DiningDate = diningDate;
    }

    public Meal(Guid id, DateOnly diningDate) : base(id)
    {
        DiningDate = diningDate;
    }

    public void RemoveAllCourses()
    {
        _courses = new List<Course>();
    }

    /// <exception cref="DomainValidationException"></exception>
    public void AddCourse(int index, bool mainCourse, int attendees, Guid dishId)
    {
        if (mainCourse)
            ValidateNoExistingMainCourse();

        Course course = new(index, mainCourse, attendees, dishId);
        AddCourse(course);
    }

    private void ValidateNoExistingMainCourse()
    {
        if (_courses.Where(c => c.MainCourse).Any())
            throw new DomainValidationException("There may only be one main course.");
    }

    private void AddCourse(Course course)
    {
        Course? courseAtSameIndex = CourseAtSameIndex(course.Index);
        if (courseAtSameIndex != null)
            ShiftIndexFor(courseAtSameIndex);

        _courses.Add(course);
    }

    private Course? CourseAtSameIndex(int index)
    {
        var conflictingCourse = Courses.Where(c => c.Index == index);
        if (conflictingCourse != null && conflictingCourse.Any())
            return conflictingCourse.First();

        return null;
    }

    private void ShiftIndexFor(Course course)
    {
        _courses.Remove(course);
        AddCourse(course with { Index = course.Index + 1 });
    }
}
