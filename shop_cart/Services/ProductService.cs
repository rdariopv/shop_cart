// Services/ProductService.cs
using System.Net.Http.Json;
using shop_cart.Models;

namespace shop_cart.Services
{
    public class ProductService
    {
        private readonly HttpClient _http;
        public ProductService(HttpClient http)
        {
            _http = http;
        }
        public async Task<List<Product>> GetProductsAsync()
        {
            return await _http.GetFromJsonAsync<List<Product>>("products")
                    ?? new List<Product>();
        }
        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<Product>($"products/{id}");
        }
        public async Task<List<int>> GetProductListImagesAsync(string productId)
        {
            return await _http.GetFromJsonAsync<List<int>>(
                $"products/products/{productId}/images")
                ?? new List<int>();
        }

        public async Task<ProductFilterResult?> SearchProductsAsync(ProductFilterModel filters)
        {
            try
            {
                // Si no hay ningún filtro activo, usa el endpoint GET que ya funciona
                bool sinFiltros = string.IsNullOrWhiteSpace(filters.SearchTerm)
                               && string.IsNullOrWhiteSpace(filters.Categoria)
                               && string.IsNullOrWhiteSpace(filters.Marca)
                               && !filters.PrecioMin.HasValue
                               && !filters.PrecioMax.HasValue;

                if (sinFiltros)
                {
                    var todos = await _http.GetFromJsonAsync<List<Product>>("products");
                    return new ProductFilterResult
                    {
                        Items = todos ?? new(),
                        TotalItems = todos?.Count ?? 0,
                        Page = 1,
                        PageSize = todos?.Count ?? 0,
                        TotalPages = 1
                    };
                }

                // Si hay filtros, usa el POST /api/products/search
                var response = await _http.PostAsJsonAsync("products/search", filters);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<ProductFilterResult>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProductService] SearchError: {ex.Message}");
                return null;
            }
        }

        public async Task<List<string>> GetCategoriasAsync()
        {
            try { return await _http.GetFromJsonAsync<List<string>>("products/categorias") ?? new(); }
            catch { return new(); }
        }

        public async Task<List<string>> GetMarcasAsync()
        {
            try { return await _http.GetFromJsonAsync<List<string>>("products/marcas") ?? new(); }
            catch { return new(); }
        }
    }
}
