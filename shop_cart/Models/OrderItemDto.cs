namespace shop_cart.Models
{
    public class OrderItemDto
    {
        public string ProductId { get; set; } = "";
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
    }
}
