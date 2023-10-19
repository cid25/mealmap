using Mealmap.Api.DataTransferObjects;
using Swashbuckle.AspNetCore.Filters;


namespace Mealmap.Api.Swagger;

public class MealRequestExampleWithoutIdAndEtag : IExamplesProvider<MealDTO>
{
    public MealDTO GetExamples()
    {
        return new MealDTO()
        {
            DiningDate = DateOnly.FromDateTime(DateTime.Now),
            Courses = new[] {
                new CourseDTO()
                {
                    Index = 1,
                    DishId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                    MainCourse = true,
                    Attendees = 2
                },
                new CourseDTO()
                {
                    Index = 2,
                    DishId = new Guid("455b6bc6-5947-402d-be8c-b2b57ca6984d"),
                    MainCourse = false,
                    Attendees = 2
                },
            }
        };
    }
}
