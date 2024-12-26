using MediatR;
using microservices.cart.api.Const;
using microservices.cart.api.Dtos;
using microservices.shared;
using microservices.shared.Services;
using Microsoft.Extensions.Caching.Distributed;
using System.Net;
using System.Text.Json;
using microservices.shared.Extensions;
using microservices.shared.Filters;
using static microservices.cart.api.Features.Cart.Operations.CartAddItemEndpoint;
using microservices.cart.api.Data;

namespace microservices.cart.api.Features.Cart.Operations
{
    public static class CartDeleteItemEndpoint
    {
        //COMMAND
        public record CartDeleteItemCommand(Guid Id) : IRequest<ServiceResult>;

        //HANDLER
        public class CartDeleteItemHandler(IDistributedCache _cache, IIdentityService _identityService) : IRequestHandler<CartDeleteItemCommand, ServiceResult>
        {
            public async Task<ServiceResult> Handle(CartDeleteItemCommand request, CancellationToken cancellationToken)
            {
                Guid userId = _identityService.GetUserId;
                var cacheKey = string.Format(BasketConst.BasketCacheKey, userId);

                var basketAsStr = await _cache.GetStringAsync(cacheKey, cancellationToken);
                if (string.IsNullOrEmpty(basketAsStr))
                    return ServiceResult.Error("Cart not found", HttpStatusCode.NotFound);

                var currentBasket = JsonSerializer.Deserialize<Basket>(basketAsStr);
                if(currentBasket is null)
                    return ServiceResult.Error("Cart is empty", HttpStatusCode.NotFound);

                var existingItem = currentBasket.basketItems.FirstOrDefault(x => x.courseId == request.Id);
                if (existingItem is null)
                    return ServiceResult.Error("Cart item not found", HttpStatusCode.NotFound);

                currentBasket.basketItems.Remove(existingItem);
                basketAsStr = JsonSerializer.Serialize(currentBasket);
                await _cache.SetStringAsync(cacheKey, basketAsStr);
                return ServiceResult.SuccessAsNoContext();
            }
        }

        //ENDPOINT
        public static RouteGroupBuilder CartDeleteItemGroupItemEndpoint(this RouteGroupBuilder group)
        {
            group.MapDelete("/item/{id:guid}",
                async (Guid id, IMediator mediator) => (await mediator.Send(new CartDeleteItemCommand(id)))
                .ToGenericResult())
                .MapToApiVersion(1, 0);

            return group;
        }
    }
}
