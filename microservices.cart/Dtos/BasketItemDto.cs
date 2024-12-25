namespace microservices.cart.api.Dtos
{
    public record BasketItemDto(
        Guid courseId,
        string courseName,
        string imageUrl,
        decimal price,
        decimal? discountAppliedPrice);
}
