using Blazored.LocalStorage;
using shop_cart.Models;

namespace shop_cart.Services
{
    public class CartStorage
    {
        private const string CART_KEY = "shopping_cart";
        private readonly ILocalStorageService _localStorage;

        public CartStorage(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task<List<CartItem>> LoadAsync()
        {
            try
            {
                return await _localStorage.GetItemAsync<List<CartItem>>(CART_KEY)
                       ?? new List<CartItem>();
            }
            catch
            {
                // Return empty cart if local storage access fails (robust prerender behavior)
                return new List<CartItem>();
            }
        }

        public async Task SaveAsync(List<CartItem> items)
        {
            try
            {
                await _localStorage.SetItemAsync(CART_KEY, items);
            }
            catch
            {
                // Ignore save errors to avoid breaking the UI if storage is unavailable
            }
        }
    }
}
