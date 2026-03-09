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
        private bool _initialized=false;

        private readonly ILocalStorageService _localStorage;
        private List<CartItem> items = new List<CartItem>();

        public event Action? OnChange;

        public int CarritoCantidad => items.Sum(x => x.Quantity);

        private readonly SemaphoreSlim _lock = new(1, 1);

        public IReadOnlyList<CartItem> GetCartItems() => items.AsReadOnly();
        public CartService(ILocalStorageService arg_localStorage)
        {
            _localStorage = arg_localStorage;
        }

        // 🔹 Cargar carrito al iniciar la app (llamar desde App.razor o MainLayout)
        public async Task InitializeAsync()
        {
            if (_initialized) return;
            
            await _lock.WaitAsync();

            try {
                if (_initialized)
                    return;

                var stored = await _localStorage.GetItemAsync<List<CartItem>>(CART_KEY);
                if (stored != null)
                {
                    items = stored;
                    //items.Clear();
                    //items.AddRange(stored);
                }

                _initialized = true;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cart initialization error: {ex.Message}");
            }
            finally { 
                _lock.Release(); 
            }
            NotifyStateChanged();
        }
 
        public async Task AgregarProducto(Product producto, int cantidad)
        {
            //if (_initialized) await InitializeAsync();

            await InitializeAsync();

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
            
            Console.WriteLine($"Carrito ahora tiene {CarritoCantidad} items");
            await PersistAsync();
        }
       
      
      
        public int GetCartCount() => CarritoCantidad;
        private void NotifyStateChanged() => OnChange?.Invoke();


        // 🔹 Guardar + notificar (punto único)
        private async Task PersistAsync()
        {
            try
            {
                await _localStorage.SetItemAsync(CART_KEY, items);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving cart: {ex.Message}");
            }

            NotifyStateChanged();
        }

        public string GenerateQrAsBase64(string content)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            var qrBytes = qrCode.GetGraphic(20);
            return "data:image/png;base64," + Convert.ToBase64String(qrBytes);
        }

       
        public async Task IncrementarAsync(CartItem item)
        {
            await InitializeAsync();
            var existing = items.FirstOrDefault(x => x.ProductItem.Id == item.ProductItem.Id);
            if (existing == null) return;

            existing.Quantity++;
            await PersistAsync();
        }
        public async Task DecrementarAsync(CartItem item)
        {
            await InitializeAsync();

            var existing = items.FirstOrDefault(x => x.ProductItem.Id == item.ProductItem.Id);
            if (existing == null) return;

            if (existing.Quantity > 1)
                existing.Quantity--;
            else
                items.Remove(existing);

            await PersistAsync();
        }
        public async Task RemoveItemAsync(CartItem item)
        {
            await InitializeAsync();
            items.Remove(item);
            await PersistAsync();
        }
        public async Task ClearAsync()
        {
            items.Clear();
            await _localStorage.RemoveItemAsync(CART_KEY);
            NotifyStateChanged();
        }

    }

}
