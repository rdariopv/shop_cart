// Services/CartService.cs
using QRCoder;
using shop_cart.Models;
namespace shop_cart.Services
{


    public class CartService
    {
        private readonly CartState _state;
        private readonly CartStorage _storage;

        public CartService(CartState state, CartStorage storage)
        {
            _state = state;
            _storage = storage;
        }

        public event Action? OnChange
        {
            add { _state.OnChange += value; }
            remove { _state.OnChange -= value; }
        }

        public int Count => _state.Count;

        public IReadOnlyList<CartItem> Items => _state.Items;

        public async Task InitializeAsync()
        {
            var items = await _storage.LoadAsync();
            _state.SetItems(items);
        }

        public async Task AddProduct(Product product, int quantity)
        {
            _state.Add(product, quantity);
            await _storage.SaveAsync(_state.Items.ToList());
        }

        public async Task IncrementAsync(CartItem item)
        {
            _state.Increment(item);
            await _storage.SaveAsync(_state.Items.ToList());
        }

        public async Task DecrementAsync(CartItem item)
        {
            _state.Decrement(item);
            await _storage.SaveAsync(_state.Items.ToList());
        }

        public async Task RemoveItemAsync(CartItem item)
        {
            _state.Remove(item);
            await _storage.SaveAsync(_state.Items.ToList());
        }

        public async Task ClearAsync()
        {
            _state.Clear();
            await _storage.SaveAsync(new List<CartItem>());
        }
        public int GetCartCount() => _state.Items.ToList().Count;
       
        

        public string GenerateQrAsBase64(string content)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            var qrBytes = qrCode.GetGraphic(20);
            return "data:image/png;base64," + Convert.ToBase64String(qrBytes);
        }

       
      
        
       

    }

}
