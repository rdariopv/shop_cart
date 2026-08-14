namespace shop_cart.Models
{
    public class OrderResponse
    {
        public bool Success { get; set; }
        public string OrderId { get; set; } = "";
        public string Message { get; set; } = "";
        public decimal Total { get; set; }
    }
}
