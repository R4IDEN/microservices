namespace microservices.cart.api.Data
{
    public class Basket
    {
        public Guid userId { get; set; }
        public List<BasketItem> basketItems { get; set; } = new();

        public float? discountRate { get; set; }
        public string? coupon { get; set; }

        public Basket(Guid UserId, List<BasketItem> BasketItems)
        {
            userId = UserId;
            basketItems = BasketItems;
        }
        public Basket()
        {
            
        }

        //computed properties
        public bool isApplyDiscount => discountRate is > 0 && !string.IsNullOrEmpty(coupon);

        public decimal TotalPrice => basketItems.Sum(x => x.price);

        public decimal? TotalPriceWithAppliedDiscount => isApplyDiscount ? basketItems.Sum(x => x.discountAppliedPrice) : TotalPrice;

        public void ApplyDiscount(string Coupon, float DiscountRate)
        {
            coupon = Coupon;
            discountRate = DiscountRate;

            //tüm ürünlerin fiyatlarını güncelle
            foreach (var item in basketItems)
            {
                item.discountAppliedPrice = item.price - (item.price * (decimal)discountRate.Value);
            }
        }

        public void ClearDiscount()
        {
            coupon = null;
            discountRate = null;
            //tüm ürünlerin fiyatlarını güncelle
            foreach (var item in basketItems)
            {
                item.discountAppliedPrice = null;
            }
        }
    }
}
