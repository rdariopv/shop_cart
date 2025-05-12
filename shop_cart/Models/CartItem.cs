namespace shop_cart.Models
{
    public class CartItem
    {
        public Product ProductItem { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
