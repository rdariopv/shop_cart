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
            return await _http.GetFromJsonAsync<List<Product>>("api/products")
                    ?? new List<Product>();
        }
        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<Product>($"api/products/{id}");
        }
        public async Task<List<int>> GetProductListImagesAsync(string productId)
        {
            return await _http.GetFromJsonAsync<List<int>>(
                $"api/products/products/{productId}/images")
                ?? new List<int>();
        }
    }
}
