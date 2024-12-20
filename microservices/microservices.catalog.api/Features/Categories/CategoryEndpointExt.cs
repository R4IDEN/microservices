using microservices.catalog.api.Features.Categories.Operations;
using microservices.catalog.api.Features.Courses.Operations;

namespace microservices.catalog.api.Features.Categories
{
    public static class CategoryEndpointExt
    {
        public static void AddCategoryGroupEndpointExt(this WebApplication app)
        {
            app.MapGroup("api/categories")
                .WithTags("Categories")
                .CreateCategoryGroupItemEndpoint()
                .GetAllCategoriesGroupItemEndpoint()
                .GetCategoryByIdGroupItemEndpoint();
        }
    }
}
