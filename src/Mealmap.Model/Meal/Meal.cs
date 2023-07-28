using Mealmap.Domain.Exceptions;

namespace Mealmap.Domain.MealAggregate;

public class Meal
{
    public Guid Id { get; }

    public DateOnly DiningDate { get; }

    public ICollection<Course> Courses { get; }

    internal Meal(DateOnly diningDate)
    {
        Id = Guid.NewGuid();
        DiningDate = diningDate;
        Courses = new List<Course>();
    }

    internal Meal(Guid id, DateOnly diningDate)
    {
        Id = id;
        DiningDate = diningDate;
        Courses = new List<Course>();
    }

    /// <exception cref="DomainValidationException"></exception>
    internal void AddCourse(int index, bool mainCourse, Guid dishId)
    {
        if (mainCourse)
            ValidateNoMainCourse();

        Course course = new(index, mainCourse, dishId);
        AddCourse(course);
    }

    private void ValidateNoMainCourse()
    {
        if (Courses.Where(c => c.MainCourse).Any())
            throw new DomainValidationException("There may only be one main course.");
    }

    private void AddCourse(Course course)
    {
        Course? courseAtSameIndex = CourseAtSameIndex(course.Index);
        if (courseAtSameIndex != null)
            ShiftIndexFor(courseAtSameIndex);

        Courses.Add(course);
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
        Courses.Remove(course);
        AddCourse(course with { Index = course.Index + 1 });
    }
}
