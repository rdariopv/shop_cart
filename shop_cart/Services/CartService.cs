// Services/CartService.cs
using System.Collections.Generic;
using System;
using shop_cart.Models;
using QRCoder;
using Blazored.LocalStorage;
namespace shop_cart.Services
{
    

    public class CartService
    {
        private const string CART_KEY = "shopping_cart";

        private readonly ILocalStorageService _localStorage;

        private List<CartItem> items = new List<CartItem>();
        public event Action OnChange;

        private readonly HttpClient _httpClient;
        private List<Product> availableProducts = new List<Product>();

        public int CarritoCantidad { get; private set; } = 0;

        private bool _initialized;



        public CartService(ILocalStorageService arg_localStorage)
        {
            //_httpClient = httpClient;
            _localStorage = arg_localStorage;
        }

        // 🔹 Cargar carrito al iniciar la app (llamar desde App.razor o MainLayout)
        public async Task InitializeAsync()
        {
            if (_initialized) return;
            items = await _localStorage.GetItemAsync<List<CartItem>>(CART_KEY)
                    ?? new List<CartItem>();

            UpdateCount();
            _initialized = true;
            NotifyStateChanged();
        }
        //public void AgregarProducto(Product producto, int cantidad)
        public async Task AgregarProducto(Product producto, int cantidad)
        {
            var existingItem = items.FirstOrDefault(i => i.ProductItem.Id  == producto.Id  );
            //CarritoCantidad += cantidad;

            if (existingItem != null)
            {
                existingItem.Quantity += cantidad;
            }
            else
            {
                items.Add(new CartItem
                {
                    ProductItem = producto,
                    Name = producto.Nombre,
                    Price = producto.Precio,
                    Quantity = cantidad
                });
            }
            CarritoCantidad= items.Sum(i => i.Quantity);
            Console.WriteLine($"Carrito ahora tiene {CarritoCantidad} items");
            await SaveCartAsync();
            NotifyStateChanged();
        }
        public async Task RemoveItem(CartItem item)
        {
            items.Remove(item);
            CarritoCantidad = items.Sum(i => i.Quantity);
            NotifyStateChanged();
        }
        public List<CartItem> GetCartItems()
        {
            return items;
        }

        //public int GetCartCount()
        //{
        //    return items.Sum(i => i.Quantity);
        //}
        public int GetCartCount() => CarritoCantidad;
        private void NotifyStateChanged() => OnChange?.Invoke();

        public string GenerateQrAsBase64(string content)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            var qrBytes = qrCode.GetGraphic(20);
            return "data:image/png;base64," + Convert.ToBase64String(qrBytes);
        }

        private void UpdateCount()
        {
            CarritoCantidad = items.Sum(i => i.Quantity);
        }

        // 🔹 Nuevo método para obtener productos desde la API
        public async Task<List<Product>> FetchProductsAsync()
        {
            try
            {
                availableProducts = await _httpClient.GetFromJsonAsync<List<Product>>("api/product") ?? new();
                return availableProducts;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al obtener productos: {ex.Message}");
                return new List<Product>();
            }
        }

        private async Task SaveCartAsync()
        {
            await _localStorage.SetItemAsync(CART_KEY, items);
        }

        public async Task LoadCartAsync()
        {
            items = await _localStorage.GetItemAsync<List<CartItem>>(CART_KEY)
                    ?? new List<CartItem>();

            NotifyStateChanged();
        }

    }

}
