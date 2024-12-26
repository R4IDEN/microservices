using MediatR;
using microservices.cart.api.Const;
using microservices.cart.api.Data;
using microservices.shared;
using microservices.shared.Extensions;
using microservices.shared.Services;
using Microsoft.Extensions.Caching.Distributed;
using System.Net;
using System.Security.Principal;
using System.Text.Json;

namespace microservices.cart.api.Features.Cart.Operations
{
    public static class ApplyDiscountCouponEndpoint
    {
        //COMMAND
        public record ApplyDiscountCouponCommand(string Coupon, float Rate) : IRequest<ServiceResult>;

        //HANDLER
        public class ApplyDiscountCouponHandler(IDistributedCache _cache, IIdentityService _service) : IRequestHandler<ApplyDiscountCouponCommand, ServiceResult>
        {
            public async Task<ServiceResult> Handle(ApplyDiscountCouponCommand request, CancellationToken cancellationToken)
            {
                var cacheKey = string.Format(BasketConst.BasketCacheKey, _service.GetUserId);

                var basketAsJson = await _cache.GetStringAsync(cacheKey, cancellationToken);
                if (string.IsNullOrEmpty(basketAsJson))
                    return ServiceResult.Error("Cart not found", HttpStatusCode.NotFound);

                var basket = JsonSerializer.Deserialize<Basket>(basketAsJson);
                if (basket is null)
                    return ServiceResult.Error("Cart is empty", HttpStatusCode.NotFound);

                basket.ApplyDiscount(request.Coupon, request.Rate);

                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(basket), cancellationToken);
                return ServiceResult.SuccessAsNoContext();
            }
        }

        //ENDPOINT
        public static RouteGroupBuilder ApplyDiscountCouponGroupItemEndpoint(this RouteGroupBuilder group)
        {
            group.MapPost("/apply-discount-coupon",
                async (IMediator mediator, ApplyDiscountCouponCommand command) => (await mediator.Send(command))
                .ToGenericResult())
                .MapToApiVersion(1, 0);
            return group;
        }
    }
}
