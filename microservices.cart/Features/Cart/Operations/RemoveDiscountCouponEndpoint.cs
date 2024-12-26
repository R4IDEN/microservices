using MediatR;
using microservices.shared.Services;
using microservices.shared;
using Microsoft.Extensions.Caching.Distributed;
using System.Net;
using System.Text.Json;
using microservices.cart.api.Const;
using microservices.cart.api.Data;
using microservices.shared.Extensions;

namespace microservices.catalog.api.Features.Courses.Operations
{
    public static class RemoveDiscountCouponEndpoint
    {
        //COMMAND
        public record RemoveDiscountCouponCommand() : IRequest<ServiceResult>;

        //HANDLER
        public class RemoveDiscountCouponHandler(IDistributedCache _cache, IIdentityService _service) : IRequestHandler<RemoveDiscountCouponCommand, ServiceResult>
        {
            public async Task<ServiceResult> Handle(RemoveDiscountCouponCommand request, CancellationToken cancellationToken)
            {
                var cacheKey = string.Format(BasketConst.BasketCacheKey, _service.GetUserId);

                var basketAsJson = await _cache.GetStringAsync(cacheKey, cancellationToken);
                if (string.IsNullOrEmpty(basketAsJson))
                    return ServiceResult.Error("Cart not found", HttpStatusCode.NotFound);

                var basket = JsonSerializer.Deserialize<Basket>(basketAsJson);
                if (basket is null)
                    return ServiceResult.Error("Cart is empty", HttpStatusCode.NotFound);

                basket.ClearDiscount();

                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(basket), cancellationToken);
                return ServiceResult.SuccessAsNoContext();
            }
        }

        //ENDPOINT
        public static RouteGroupBuilder RemoveDiscountCouponGroupItemEndpoint(this RouteGroupBuilder group)
        {
            group.MapPut("/remove-discount-coupon",
                async (IMediator mediator, RemoveDiscountCouponCommand command) => (await mediator.Send(command))
                .ToGenericResult())
                .MapToApiVersion(1, 0);
            return group;
        }
    }
}
