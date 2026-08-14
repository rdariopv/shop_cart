namespace shop_cart.Models
{
    public class ProductFilterResult
    {
        public List<Product> Items { get; set; } = new();
        public int TotalItems { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
