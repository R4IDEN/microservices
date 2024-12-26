using System.Text.Json.Serialization;

namespace microservices.cart.api.Dtos
{
    public record BasketDto
    {
        public List<BasketItemDto> basketItems { get; set; } = new();

        public BasketDto(List<BasketItemDto> BasketItems)
        {
            basketItems = BasketItems;
        }

        public BasketDto()
        {
        }
    }
}
