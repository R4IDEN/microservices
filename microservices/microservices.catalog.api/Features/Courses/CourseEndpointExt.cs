using microservices.catalog.api.Features.Categories.Operations;
using microservices.catalog.api.Features.Courses.Operations;

namespace microservices.catalog.api.Features.Courses
{
    public static class CourseEndpointExt
    {
        public static void AddCourseGroupEndpointExt(this WebApplication app)
        {
            app.MapGroup("api/courses")
                .WithTags("Courses")
                .CreateCourseGroupItemEndpoint()
                .GetAllCoursesGroupItemEndpoint()
                .GetCourseByIdGroupItemEndpoint()
                .UpdateCourseGroupItemEndpoint()
                .DeleteCourseGroupItemEndpoint();
        }
    }
}
