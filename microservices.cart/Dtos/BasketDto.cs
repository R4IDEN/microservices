using System.Text.Json.Serialization;

namespace microservices.cart.api.Dtos
{
    public record BasketDto
    {
        [JsonIgnore] public Guid userId { get; init; }
        public List<BasketItemDto> basketItems { get; set; } = new();

        public BasketDto(Guid UserId, List<BasketItemDto> BasketItems)
        {
            userId = UserId;
            basketItems = BasketItems;
        }

        public BasketDto()
        {
        }
    }
}
