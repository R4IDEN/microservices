using AutoMapper;
using MediatR;
using microservices.cart.api.Const;
using microservices.cart.api.Data;
using microservices.cart.api.Dtos;
using microservices.shared;
using microservices.shared.Extensions;
using microservices.shared.Services;
using Microsoft.Extensions.Caching.Distributed;
using System.Net;
using System.Text.Json;

namespace microservices.cart.api.Features.Cart.Operations
{
    public static class CartGetCartEndpoint
    {
        //QUERY
        public record CartGetCartQuery : IRequest<ServiceResult<BasketDto>>;

        //HANDLER
        public class CartGetCartHandler(IDistributedCache _cache, IIdentityService _service, IMapper _mapper) : IRequestHandler<CartGetCartQuery, ServiceResult<BasketDto>>
        {
            public async Task<ServiceResult<BasketDto>> Handle(CartGetCartQuery request, CancellationToken cancellationToken)
            {
                var cacheKey = string.Format(BasketConst.BasketCacheKey, _service.GetUserId);

                var basketAsStr = await _cache.GetStringAsync(cacheKey, cancellationToken);
                if (string.IsNullOrEmpty(basketAsStr))
                    return ServiceResult<BasketDto>.Error("Cart not found", HttpStatusCode.NotFound);

                var basket = JsonSerializer.Deserialize<Basket>(basketAsStr);
                if (basket is null)
                    return ServiceResult<BasketDto>.Error("Cart is empty", HttpStatusCode.NotFound);

                var basketDto = _mapper.Map<BasketDto>(basket);

                return ServiceResult<BasketDto>.SuccessAsNoContext(basketDto);
            }
        }

        //ENDPOINT
        public static RouteGroupBuilder CartGetCartGroupItemEndpoint(this RouteGroupBuilder group)
        {
            group.MapGet("/",
                async (IMediator mediator) => (await mediator.Send(new CartGetCartQuery()))
                .ToGenericResult())
                .MapToApiVersion(1, 0);
            return group;
        }
    }
}
