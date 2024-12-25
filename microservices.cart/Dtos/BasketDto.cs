namespace microservices.cart.api.Dtos
{
    public record BasketDto(
        Guid userId,
        List<BasketItemDto> basketItems);
}
