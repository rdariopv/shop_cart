using System.Dynamic;
namespace shop_cart.Models
{
    public class OrderRequest
    {
        public CustomerInfo Customer { get; set; } = new();
        public string PaymentMethod { get; set; } = "card";
        public CardInfo? Card { get; set; }
        public string? CashNote { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
        public decimal Total { get; set; }
    }
}