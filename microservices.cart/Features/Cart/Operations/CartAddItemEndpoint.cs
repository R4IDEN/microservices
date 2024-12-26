using FluentValidation;
using MediatR;
using microservices.cart.api.Const;
using microservices.cart.api.Dtos;
using microservices.shared;
using microservices.shared.Filters;
using microservices.shared.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using System.Net;
using System.Text.Json;
using microservices.shared.Services;

namespace microservices.cart.api.Features.Cart
{
    public static class CartAddItemEndpoint
    {
        //COMMAND
        public record CartAddItemCommand(
            Guid CourseId,
            string CourseName,
            decimal CoursePrice,
            string CourseImageUrl
            ) : IRequest<ServiceResult>;

        //VALIDATOR
        public class CartAddItemValidator : AbstractValidator<CartAddItemCommand>
        {
            public CartAddItemValidator()
            {
                RuleFor(x => x.CourseId).NotEmpty().WithMessage("CourseId is required");
                RuleFor(x => x.CourseName).NotEmpty().WithMessage("Course name is required");
                RuleFor(x => x.CoursePrice).GreaterThan(0); ;
            }
        }

        //HANDLER
        public class CartAddItemHandler(IDistributedCache _cache, IIdentityService _identityService) : IRequestHandler<CartAddItemCommand, ServiceResult>
        {
            public async Task<ServiceResult> Handle(CartAddItemCommand request, CancellationToken cancellationToken)
            {
                //To do : Buradaki userId'yi nasıl alacağız?
                Guid userId = _identityService.GetUserId;
                var cacheKey = string.Format(BasketConst.BasketCacheKey, userId);

                var basketAsStr = await _cache.GetStringAsync(cacheKey);
                BasketDto? currentBasket;

                var cartItem = new BasketItemDto(
                    request.CourseId,
                    request.CourseName,
                    request.CourseImageUrl,
                    request.CoursePrice,
                    null);

                if (string.IsNullOrEmpty(basketAsStr))
                    currentBasket = new BasketDto(userId, [cartItem]);

                else
                {
                    currentBasket = JsonSerializer.Deserialize<BasketDto>(basketAsStr);

                    if (currentBasket is null)
                        return ServiceResult.Error("An error occured while deserializing cart", HttpStatusCode.InternalServerError);

                    var existingCartItem = currentBasket.basketItems.FirstOrDefault(x => x.courseId == request.CourseId);

                    if (existingCartItem is not null)
                        //Burada aynı ürün varsa ne yapacağız?
                        currentBasket.basketItems.Remove(existingCartItem);

                    currentBasket.basketItems.Add(cartItem);
                }

                basketAsStr = JsonSerializer.Serialize(currentBasket);
                await _cache.SetStringAsync(cacheKey, basketAsStr, cancellationToken);
                return ServiceResult.SuccessAsNoContext();
            }
        }

        //ENDPOINT
        public static RouteGroupBuilder CartAddItemGroupItemEndpoint(this RouteGroupBuilder group)
        {
            group.MapPost("/item/",
                async (CartAddItemCommand cmd, IMediator mediator) => (await mediator.Send(cmd))
                .ToGenericResult())
                .MapToApiVersion(1, 0)
                .AddEndpointFilter<ValidationFilter<CartAddItemCommand>>();

            return group;
        }
    }
}
