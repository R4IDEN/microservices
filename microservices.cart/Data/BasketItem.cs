namespace microservices.cart.api.Data
{
    public class BasketItem
    {
        public Guid courseId { get; set; }
        public string courseName { get; set; } = default!;
        public string? imageUrl { get; set; }
        public decimal price { get; set; }
        public decimal? discountAppliedPrice { get; set; }

        public BasketItem(decimal? discountAppliedPrice, decimal price, string? imageUrl, string courseName, Guid courseId)
        {
            this.discountAppliedPrice = discountAppliedPrice;
            this.price = price;
            this.imageUrl = imageUrl;
            this.courseName = courseName;
            this.courseId = courseId;
        }
    }
}
