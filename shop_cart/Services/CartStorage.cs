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
            return await _localStorage.GetItemAsync<List<CartItem>>(CART_KEY)
                   ?? new List<CartItem>();
        }

        public async Task SaveAsync(List<CartItem> items)
        {
            await _localStorage.SetItemAsync(CART_KEY, items);
        }
    }
}
