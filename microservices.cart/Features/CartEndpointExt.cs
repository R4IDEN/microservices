using Asp.Versioning.Builder;
using microservices.cart.api.Features.Cart;
using microservices.cart.api.Features.Cart.Operations;

namespace microservices.cart.api.Features
{
    public static class CartEndpointExt
    {
        public static void AddCartGroupEndpointExt(this WebApplication app, ApiVersionSet apiVersionSet)
        {
            app.MapGroup("api/v{version:apiVersion}/cart")
                .WithTags("Cart")
                .WithApiVersionSet(apiVersionSet)
                .CartAddItemGroupItemEndpoint()
                .CartDeleteItemGroupItemEndpoint();
        }
    }
}
