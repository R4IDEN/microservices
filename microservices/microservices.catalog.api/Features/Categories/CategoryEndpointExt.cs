using Asp.Versioning.Builder;
using microservices.catalog.api.Features.Categories.Operations;
using microservices.catalog.api.Features.Courses.Operations;

namespace microservices.catalog.api.Features.Categories
{
    public static class CategoryEndpointExt
    {
        public static void AddCategoryGroupEndpointExt(this WebApplication app, ApiVersionSet apiVersionSet)
        {
            app.MapGroup("api/v{version:apiVersion}/categories")
                .WithTags("Categories")
                .WithApiVersionSet(apiVersionSet)
                .CreateCategoryGroupItemEndpoint()
                .GetAllCategoriesGroupItemEndpoint()
                .GetCategoryByIdGroupItemEndpoint();
        }
    }
}
