using AutoMapper;

namespace microservices.cart.api.Features.Cart
{
    public class CartMapping : Profile
    {
        public CartMapping()
        {
            CreateMap<Data.Basket, Dtos.BasketDto>().ReverseMap();
            CreateMap<Data.BasketItem, Dtos.BasketItemDto>().ReverseMap();
        }
    }
}
