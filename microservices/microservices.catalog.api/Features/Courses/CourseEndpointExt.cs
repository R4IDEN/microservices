using Asp.Versioning.Builder;
using microservices.catalog.api.Features.Categories.Operations;
using microservices.catalog.api.Features.Courses.Operations;

namespace microservices.catalog.api.Features.Courses
{
    public static class CourseEndpointExt
    {
        public static void AddCourseGroupEndpointExt(this WebApplication app, ApiVersionSet apiVersionSet)
        {
            app.MapGroup("api/v{version:apiVersion}/courses")
                .WithTags("Courses")
                .WithApiVersionSet(apiVersionSet)
                .CreateCourseGroupItemEndpoint()
                .GetAllCoursesGroupItemEndpoint()
                .GetCourseByIdGroupItemEndpoint()
                .UpdateCourseGroupItemEndpoint()
                .DeleteCourseGroupItemEndpoint()
                .GetCoursesByUserIdGroupItemEndpoint();
        }
    }
}
