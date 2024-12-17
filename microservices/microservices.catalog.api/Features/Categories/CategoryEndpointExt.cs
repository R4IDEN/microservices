using microservices.catalog.api.Features.Categories.TXNS;
using microservices.shared.Filters;

namespace microservices.catalog.api.Features.Categories
{
    public static class CategoryEndpointExt
    {
        public static void AddCategoryGroupEndpointExt(this WebApplication app)
        {
            app.MapGroup("api/categories")
                .CreateCategoryGroupItemEndpoint();
        }
    }
}
