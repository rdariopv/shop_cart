namespace shop_cart.Models
{
    public class ProductFilterModel
    {
        public string? SearchTerm { get; set; }
        public string? Categoria { get; set; }
        public string? Marca { get; set; }
        public decimal? PrecioMin { get; set; }
        public decimal? PrecioMax { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
